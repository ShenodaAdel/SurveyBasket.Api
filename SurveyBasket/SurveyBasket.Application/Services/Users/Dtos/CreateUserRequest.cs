namespace SurveyBasket.Application.Services.Users.Dtos
{
    public record CreateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        IList<string> Roles
        );
}
