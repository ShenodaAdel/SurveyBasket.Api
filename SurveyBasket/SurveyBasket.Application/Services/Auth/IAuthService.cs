using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<object?>> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<ApiResponse<object?>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<object?>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<object?>> RegisterAutoAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<object?>> ResendConfirmationEmailAsync(ResendConfirmationEmail request);
    }
}
