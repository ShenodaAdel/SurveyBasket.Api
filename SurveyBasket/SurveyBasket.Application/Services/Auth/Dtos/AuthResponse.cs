namespace SurveyBasket.Application.Services.Auth.Dtos
{
    public record AuthResponse (
        string Id ,
        string? Email,
        string FirstName,
        string LirstName,
        string Token,
        int ExpiresIn
        );
}
