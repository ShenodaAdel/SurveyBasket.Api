using SurveyBasket.Application.Services.Vote.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class VoteRequestValidator : AbstractValidator<VoteRequest>
    {
        public VoteRequestValidator()
        {
            RuleFor(v => v.Answers)
                .NotEmpty()
                .WithMessage("The answers are required.");

            RuleForEach(a => a.Answers)
                .SetValidator(new VoteAnswerRequestValidator());
        }
    }
}
