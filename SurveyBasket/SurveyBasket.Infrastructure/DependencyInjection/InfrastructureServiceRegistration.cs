using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Application.Services.Auth.JWT;
using SurveyBasket.Domain.Entities;
using SurveyBasket.Infrastructure.Identity;
using System.Text;

namespace SurveyBasket.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));


            // Identity setup
            var builder = services.AddIdentityCore<ApplicationUser>();
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<ApplicationDbContext>();
            builder.AddSignInManager();
            builder.AddDefaultTokenProviders();


            //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart(); // if found problem not make Application Run That 's mean stop Application

            var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            services.AddScoped<IJWTProvider, JWTProvider>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
