using Microsoft.Extensions.DependencyInjection;

namespace SurveyBasket.Application.DependencyInjection
{
    // This class is intended to be used for registering application services in the dependency injection container -> Extension Method.
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Here you can register your application services, for example:
            // services.AddScoped<IYourService, YourService>();
            return services;
        }
    }
}
