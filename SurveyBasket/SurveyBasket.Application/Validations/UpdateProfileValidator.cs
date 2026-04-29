using SurveyBasket.Application.Services.Users.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .Length(3,100).WithMessage("First name must be between 3 and 100 characters.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(3,100).WithMessage("Last name must be between 3 and 100  characters.");
        }
    }
}
