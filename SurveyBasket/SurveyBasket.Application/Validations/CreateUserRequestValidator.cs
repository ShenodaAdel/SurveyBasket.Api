using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Users.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(u => u.FirstName)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(u => u.LastName)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(u => u.Password)
                .NotEmpty()
                .Matches(RegexPatterns.PasswordPattern)
                .WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

            RuleFor(u => u.Roles)
                .NotNull()
                .NotEmpty();

            RuleFor(u => u.Roles)
                .Must(u => u.Distinct().Count() == u.Count)
                .WithMessage("Roles must be unique.")
                .When(u => u.Roles != null);
        }
    }
}
