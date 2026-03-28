using SurveyBasket.Domain.Common;

namespace SurveyBasket.Domain.Entities
{
    public sealed class Poll : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public bool IsPublished { get; set; } = false;
        public DateOnly StartsAt { get; set; }
        public DateOnly EndsAt { get; set; }
        public ICollection<Question> Questions { get; set; } = [];
    }
}
