using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Code)
               .NotEmpty().WithMessage("Code is required.");
        }
    }
}
