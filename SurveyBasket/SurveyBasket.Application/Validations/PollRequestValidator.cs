namespace SurveyBasket.Application.Validations
{
    public class PollRequestValidator : AbstractValidator<PollRequest>
    {
        public PollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.");
            
            RuleFor(x => x.Summary)
                .NotEmpty().WithMessage("Summary is required.")
                .Length(10, 1500).WithMessage("Summary must be between 10 and 1500 characters.");

            RuleFor(x => x.StarstAt)
                .NotEmpty().WithMessage("StarstAt is required")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

            RuleFor(x => x.EndsAt)
                .NotEmpty().WithMessage("EndsAt is required");

            RuleFor(x => x) // validate the while model
                .Must(HasValidDates).WithName(nameof(PollRequest.EndsAt))
                .WithMessage("{PropertyName} must be greater than or equals start date.");
        }

        private bool HasValidDates(PollRequest request)
        {
            return request.EndsAt >= request.StarstAt;
        }
    }
}
