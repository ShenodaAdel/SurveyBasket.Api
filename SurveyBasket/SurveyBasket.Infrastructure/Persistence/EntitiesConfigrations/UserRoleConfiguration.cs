using SurveyBasket.Application.Helpers;

namespace SurveyBasket.Infrastructure.Persistence.EntitiesConfigrations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    UserId = DefaultUsers.Admin.Id,
                    RoleId = DefaultRoles.Admin.Id,
                }
            );
        }
    }
}
