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
                    Id = DefaultRoles.Admin.Id,
                    Name = DefaultRoles.Admin.Name,
                    NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp,
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.User.Id,
                    Name = DefaultRoles.User.Name,
                    NormalizedName = DefaultRoles.User.Name.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.User.ConcurrencyStamp,
                    IsDefault = true,
                }
            ]);
        }
    }
}
