using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Application.Services.Auth.Filter
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {
    }
}
