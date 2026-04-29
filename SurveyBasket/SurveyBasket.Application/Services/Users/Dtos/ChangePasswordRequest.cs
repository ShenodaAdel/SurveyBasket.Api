namespace SurveyBasket.Application.Services.Users.Dtos
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
