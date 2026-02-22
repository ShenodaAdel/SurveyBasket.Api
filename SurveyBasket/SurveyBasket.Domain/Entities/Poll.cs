using SurveyBasket.Domain.Common;
using System.Security.Principal;

namespace SurveyBasket.Domain.Entities
{
    public sealed class Poll : BaseEntity
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool Ispublished { get; set; } = false;
        public DateOnly StarstAt { get; set; }
        public DateOnly EndsAt { get; set; }
    }
}
