using SurveyBasket.Application.Services.Role.Dtos;

namespace SurveyBasket.Application.Validations
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .Length(3,100).WithMessage("Role name must not exceed 100 characters.");

            RuleFor(r => r.Permissions)
                .NotNull().WithMessage("Permissions are required.")
                .NotEmpty()
                .Must(p => p.Distinct().Count() == p.Count )
                .WithMessage("You can not add dublicated permissions for the same role")
                .When(p => p.Permissions != null);
        }
    }
}
