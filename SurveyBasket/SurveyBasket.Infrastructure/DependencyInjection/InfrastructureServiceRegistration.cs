using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SurveyBasket.Application.Services.Auth.JWT;
using SurveyBasket.Infrastructure.Identity;

namespace SurveyBasket.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // Identity setup 
            var builder = services.AddIdentityCore<ApplicationUser>();
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<ApplicationDbContext>();   

            services.AddSingleton<IJWTProvider, JWTProvider>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
