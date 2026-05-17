namespace SurveyBasket.Application.Services.Users.Dtos
{
    public record UpdateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        IList<string> Roles
        );
}
