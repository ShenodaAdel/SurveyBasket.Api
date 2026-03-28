using SurveyBasket.Domain.Common;

namespace SurveyBasket.Domain.Entities
{
    public sealed class Answer : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int QuestionId { get; set; }
        // public bool IsActive { get; set; } = true;
        public Question Question { get; set; } = default!;
    }
}
