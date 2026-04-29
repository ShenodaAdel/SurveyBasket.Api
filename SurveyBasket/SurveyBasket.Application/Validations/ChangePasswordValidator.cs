using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Users.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .Matches(RegexPatterns.PasswordPattern).WithMessage("New password should be at least 8 characters and should contain lowercase, uppercase, digit, and special character.")
                .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password.");
        }
    }
}
