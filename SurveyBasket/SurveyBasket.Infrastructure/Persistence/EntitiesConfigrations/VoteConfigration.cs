namespace SurveyBasket.Infrastructure.Persistence.EntitiesConfigrations
{
    public class VoteConfigration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.HasIndex(x => new { x.PollId, x.UserId }).IsUnique(); // USer can make vote greater than one for the same poll 
        }
    
    }
}
