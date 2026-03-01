using SurveyBasket.Application.Responses;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors
                .Select(e => new ApiResponseMessage(
                    type: "error",
                    text: e.ErrorMessage,
                    field: null)))
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


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
