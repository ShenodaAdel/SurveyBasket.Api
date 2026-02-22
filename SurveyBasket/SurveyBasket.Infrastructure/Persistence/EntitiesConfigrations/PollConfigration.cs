

namespace SurveyBasket.Infrastructure.Persistence.EntitiesConfigrations
{
    public class PollConfigration : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.HasIndex(x => x.Title).IsUnique(); // i need to learn it and ask also 

            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Summary).HasMaxLength(1500);
        }
    }
}
