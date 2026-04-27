using SurveyBasket.Application.Services.Auth;
using SurveyBasket.Application.Services.Caching;
using SurveyBasket.Application.Services.Email;
using SurveyBasket.Application.Services.Notification;
using SurveyBasket.Application.Services.Question;
using SurveyBasket.Application.Services.Result;
using SurveyBasket.Application.Services.Vote;

namespace SurveyBasket.Application.DependencyInjection
{
    // This class is intended to be used for registering application services in the dependency injection container -> Extension Method.
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // mapster configuration for mapping between domain entities and DTOs -> using reflection to scan the assembly for mapping configurations.
            var config = new TypeAdapterConfig();

            config.Scan(typeof(ApplicationServiceRegistration).Assembly);

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            // add dependency for valifation of DTOs using FluentValidation -> using reflection to scan the assembly for validators.
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);



            //
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IAuthService,AuthService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
