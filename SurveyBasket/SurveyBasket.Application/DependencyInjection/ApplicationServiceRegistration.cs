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
            services.AddScoped<IPollService, PollService>();

            return services;
        }
    }
}
