using Asp.Versioning;
using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.API.Extensions;
using SurveyBasket.API.Health;
using SurveyBasket.API.Middleware;
using SurveyBasket.API.Swagger;
using SurveyBasket.Application.Responses;
using SurveyBasket.Application.Services.Email;
using SurveyBasket.Application.Services.Notification;
using SurveyBasket.Infrastructure.Persistence;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options => 
    options.AddDefaultPolicy( pollcy =>
        pollcy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!)
    )
);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value is not null && x.Value.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors
                .Select(e => new ApiResponseMessage(
                    type: "error",
                    text: e.ErrorMessage,
                    field: x.Key)))
            .ToList();

        var response = new ApiResponse<object?>(
            status: StatusCodes.Status400BadRequest,
            data: null,
            messages: errors);

        return new BadRequestObjectResult(response);
    };
});


builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

// RateLimiting configuration
builder.Services.AddRateLimiter(rateLimiteroptions =>
{
    rateLimiteroptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiteroptions.AddPolicy("ipLimit", HttpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: HttpContext.Connection.RemoteIpAddress?.ToString(),
        factory : _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 2,
            Window = TimeSpan.FromSeconds(10)
        }
    ));

    rateLimiteroptions.AddPolicy("userLimit", HttpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: HttpContext.User.GetUserId(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 2,
            Window = TimeSpan.FromSeconds(10)
        }
    ));


    rateLimiteroptions.AddConcurrencyLimiter("ConcurrencyLimiter", options =>
    {
        options.PermitLimit = 1000; // can you take 10 requests at the same time
        options.QueueLimit = 100;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // FIFO queue
        // if more than 10 requests come at the same time, they will be queued.
        // This is the maximum number of requests that can be queued. If the queue is full,
        // the request will be rejected with a 429 Too Many Requests response.
    });

    //rateLimiteroptions.AddTokenBucketLimiter("TokenBucketLimiter", options =>
    //{
    //    options.TokenLimit = 10; // maximum number of tokens in the bucket
    //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // FIFO queue
    //    options.QueueLimit = 1; // maximum number of requests that can be queued
    //    options.ReplenishmentPeriod = TimeSpan.FromSeconds(30); // time to replenish the bucket
    //    options.TokensPerPeriod = 2; // number of tokens to add to the bucket each replenishment period)
    //    options.AutoReplenishment = true; // automatically replenish the bucket
    //});

    //rateLimiteroptions.AddFixedWindowLimiter("FixedWindowLimiter", options =>
    //{
    //    options.Window = TimeSpan.FromSeconds(30); // time window for the fixed window limiter 
    //    options.PermitLimit = 10; // maximum number of requests allowed in the time window
    //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // FIFO queue
    //    options.QueueLimit = 1; // maximum number of requests that can be queued
    //});

    //rateLimiteroptions.AddSlidingWindowLimiter("SlidingWindowLimiter", options =>
    //{
    //    options.Window = TimeSpan.FromSeconds(30); // time window for the sliding window limiter
    //    options.PermitLimit = 10; // maximum number of requests allowed in the time window
    //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // FIFO queue
    //    options.QueueLimit = 1; // maximum number of requests that can be queued
    //    options.SegmentsPerWindow = 3; // number of segments in the sliding window
    //});
});

// Api Versioning configuration
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true; 
    options.ReportApiVersions = true;

    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    //options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
    //options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
    //options.ApiVersionReader = new MediaTypeApiVersionReader("x-api-version");

})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true; 

});


// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddHangfire(options =>
    {
        options.MinimumAvailableServers = 1;
    })
    .AddCheck<MailProviderHealthCheck>("Mail Provider");

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs" , new DashboardOptions
    {
        Authorization = 
        [
            new HangfireCustomBasicAuthenticationFilter
            {
                User = app.Configuration.GetValue<string>("HangfireSettings:Username")!,
                Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")!
            }
        ],
        DashboardTitle = "Survey Basket Dashboard",
        //IsReadOnlyFunc = (DashboardContext context) => true 
});

var scopedFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopedFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendNewPollsNotification(null), Cron.Daily);

// must me put before Authorization
app.UseCors();

app.UseExceptionHandler();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapHealthChecks("health",new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
