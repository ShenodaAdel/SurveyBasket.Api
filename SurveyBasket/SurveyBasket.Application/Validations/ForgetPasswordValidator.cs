using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class ForgetPasswordValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
