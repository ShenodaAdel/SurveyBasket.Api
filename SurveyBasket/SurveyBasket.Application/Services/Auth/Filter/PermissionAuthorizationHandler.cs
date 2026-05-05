using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Application.Helpers;

namespace SurveyBasket.Application.Services.Auth.Filter
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            if (context.User.Identity is not { IsAuthenticated: true } ||
                !context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type))
                return;

            context.Succeed(requirement);
            return;
        }
    }
}
