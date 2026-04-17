namespace SurveyBasket.Application.Services.Auth.Dtos
{
    public record RegisterRequest(string Email , string Password , string FirstName , string LastName);
}
