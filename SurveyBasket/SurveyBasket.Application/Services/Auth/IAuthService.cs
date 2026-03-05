namespace SurveyBasket.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<object?>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}
