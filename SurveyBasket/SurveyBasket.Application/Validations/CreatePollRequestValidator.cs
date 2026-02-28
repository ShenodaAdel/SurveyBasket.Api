namespace SurveyBasket.Application.Validations
{
    public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
    {
        public CreatePollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.");
            
            RuleFor(x => x.Summary)
                .NotEmpty().WithMessage("Summary is required.")
                .Length(10, 1500).WithMessage("Summary must be between 10 and 1500 characters.");

        }
    }
}
