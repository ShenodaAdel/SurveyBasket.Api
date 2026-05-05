using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Application.Services.Auth.Filter
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
