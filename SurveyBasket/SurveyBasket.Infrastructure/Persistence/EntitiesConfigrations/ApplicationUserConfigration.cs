using SurveyBasket.Application.Helpers;

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
                    Id = DefaultUsers.Admin.Id,
                    FirstName = "Survey Basket",
                    LastName = "Admin",
                    UserName = DefaultUsers.Admin.Email,
                    NormalizedUserName = DefaultUsers.Admin.Email.ToUpper(),
                    Email = DefaultUsers.Admin.Email,
                    NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
                    SecurityStamp = DefaultUsers.Admin.SecuirtyStamp,
                    ConcurrencyStamp = DefaultUsers.Admin.ConcurrencyStamp,
                    EmailConfirmed = true,
                    PasswordHash = DefaultUsers.Admin.PasswordHash
                });
        }

    }
}
