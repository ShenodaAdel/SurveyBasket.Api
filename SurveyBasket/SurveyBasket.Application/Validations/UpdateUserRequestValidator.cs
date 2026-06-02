using SurveyBasket.Application.Services.Users.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
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
