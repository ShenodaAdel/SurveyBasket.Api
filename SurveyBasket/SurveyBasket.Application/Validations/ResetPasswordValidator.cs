using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Reset code is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .Matches(RegexPatterns.PasswordPattern).WithMessage("Password does not meet complexity requirements.");
        }
    }
}
