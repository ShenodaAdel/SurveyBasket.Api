using SurveyBasket.Application.Helpers;
using SurveyBasket.Domain.Entities;

namespace SurveyBasket.Infrastructure.Persistence.EntitiesConfigrations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.OwnsMany(x => x.RefreshTokens)
                .ToTable("RefreshTokens")
                .WithOwner()
                .HasForeignKey("UserId");

            builder.Property(x => x.FirstName).HasMaxLength(100);
            builder.Property(x => x.LastName).HasMaxLength(100);


            var passwordHasher = new PasswordHasher<ApplicationUser>();

            builder.HasData(
                new ApplicationUser
                {
                    Id = DefaultUsers.AdminId,
                    FirstName = "Survey Basket",
                    LastName = "Admin",
                    UserName = DefaultUsers.AdminEmail,
                    NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                    Email = DefaultUsers.AdminEmail,
                    NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                    SecurityStamp = DefaultUsers.AdminSecuirtyStamp,
                    ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null!, DefaultUsers.AdminPassword)
                });
        }

    }
}
