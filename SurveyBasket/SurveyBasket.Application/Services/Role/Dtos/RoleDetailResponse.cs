namespace SurveyBasket.Application.Services.Role.Dtos
{
    public record RoleDetailResponse(string Id, string Name, bool IsDeleted, IEnumerable<string> Permissions);
}
