namespace SurveyBasket.Application.Services.Auth.Dtos
{
    public record ResetPasswordRequest(string Email, string Code, string NewPassword);
}
