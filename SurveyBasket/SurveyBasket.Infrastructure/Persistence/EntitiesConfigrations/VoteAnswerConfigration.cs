namespace SurveyBasket.Infrastructure.Persistence.EntitiesConfigrations
{
    public class VoteAnswerConfigration : IEntityTypeConfiguration<VoteAnswer>
    {
        public void Configure(EntityTypeBuilder<VoteAnswer> builder)
        {
            builder.HasIndex(x => new { x.VoteId, x.QuestionId }).IsUnique(); // the one question must choose once 

        }
    
    }
}
