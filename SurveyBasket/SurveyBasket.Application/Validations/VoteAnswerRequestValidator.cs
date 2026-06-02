using SurveyBasket.Application.Services.Vote.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
    {
        public VoteAnswerRequestValidator()
        {
            RuleFor(v => v.QuestionId)
                .GreaterThan(0)
                .WithMessage("Question Id is required and must be greater than 0.");
            RuleFor(v => v.AnswerId)
                .GreaterThan(0)
                .WithMessage("Answer Id is required and must be greater than 0.");
        }
    }
}
