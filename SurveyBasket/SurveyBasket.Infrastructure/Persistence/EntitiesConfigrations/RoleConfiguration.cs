using SurveyBasket.Application.Helpers;

namespace SurveyBasket.Infrastructure.Persistence.EntitiesConfigrations
{
    public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData([
                new ApplicationRole
                {
                    Id = DefaultRoles.AdminRoleId,
                    Name = DefaultRoles.Admin,
                    NormalizedName = DefaultRoles.Admin.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.AdminConcurrencyStamp,
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.UserRoleId,
                    Name = DefaultRoles.User,
                    NormalizedName = DefaultRoles.User.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.UserConcurrencyStamp,
                    IsDefault = true,
                }
            ]);
        }
    }
}
