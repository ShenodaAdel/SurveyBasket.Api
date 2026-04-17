using SurveyBasket.Application.Services.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
               .Matches(RegexPatterens.PasswordPattern)
               .WithMessage("Password should be at least 8 dights and and should contains Lowercase and UperCase ");

            RuleFor(x => x.FirstName)
               .NotEmpty().WithMessage("FirstName is required.")
               .Length(3,100);

            RuleFor(x => x.LastName)
               .NotEmpty().WithMessage("LastName is required.")
               .Length(3, 100);

        }
        public static class RegexPatterens
        {
            public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        }
    }
}
