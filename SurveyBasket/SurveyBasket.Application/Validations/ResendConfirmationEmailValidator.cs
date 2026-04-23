using SurveyBasket.Application.Services.Auth.Dtos;
namespace SurveyBasket.Application.Validations
{
    public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmail>
    {
        public ResendConfirmationEmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

        }

    }
}
