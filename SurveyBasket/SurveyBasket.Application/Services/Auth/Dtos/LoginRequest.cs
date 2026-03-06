namespace SurveyBasket.Application.Services.Auth.Dtos
{
    public record LoginRequest(
        string Email,
        string Password
        );
}
