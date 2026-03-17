namespace SurveyBasket.Infrastructure.Persistence.EntitiesConfigrations
{
    public class PollConfiguration : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.HasIndex(x => x.Title).IsUnique();

            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Summary).HasMaxLength(1500);

            builder.Property(x => x.CreatedById).HasMaxLength(450);

            builder.HasOne<ApplicationUser>(x => x.CreatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedById)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.UpdatedById).HasMaxLength(450);

            builder.HasOne<ApplicationUser>(x => x.UpdatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.UpdatedById)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
