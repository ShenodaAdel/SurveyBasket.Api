namespace SurveyBasket.Application.Services.Role.Dtos
{
    public record RoleRequest(string Name , IList<string> Permissions);
}
