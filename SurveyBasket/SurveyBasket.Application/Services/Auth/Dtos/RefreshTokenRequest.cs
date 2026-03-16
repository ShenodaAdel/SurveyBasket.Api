namespace SurveyBasket.Application.Services.Auth.Dtos
{
    public class RefreshTokenRequest
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
