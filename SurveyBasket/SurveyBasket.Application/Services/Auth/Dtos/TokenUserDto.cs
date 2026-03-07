namespace SurveyBasket.Application.Services.Auth.Dtos
{
    public class TokenUserDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public IList<string>? Roles { get; set; }
    }
}
