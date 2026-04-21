namespace SurveyBasket.Application.Services.Auth.Dtos
{
    public record ConfirmEmailRequest(string UserId, string Code);
}
