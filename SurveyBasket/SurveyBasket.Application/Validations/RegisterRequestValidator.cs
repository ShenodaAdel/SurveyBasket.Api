using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid");

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("Password is required.")
               .Matches(RegexPatterns.PasswordPattern)
               .WithMessage("Password should be at least 8 dights and and should contains Lowercase and UperCase ");

            RuleFor(x => x.FirstName)
               .NotEmpty().WithMessage("FirstName is required.")
               .Length(3,100);

            RuleFor(x => x.LastName)
               .NotEmpty().WithMessage("LastName is required.")
               .Length(3, 100);

        }
    }
}
