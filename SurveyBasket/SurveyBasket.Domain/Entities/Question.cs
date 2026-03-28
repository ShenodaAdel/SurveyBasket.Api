using SurveyBasket.Domain.Common;

namespace SurveyBasket.Domain.Entities
{
    public sealed class Question : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int PollId { get; set; }
        // public bool IsActive { get; set; } = true; Because the baseEntity is exist IsDeleted , we don't need IsActive
        public Poll Poll { get; set; } = default!;
        public ICollection<Answer> Answers { get; set; } = [];
    }
}
