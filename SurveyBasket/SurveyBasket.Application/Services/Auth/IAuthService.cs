using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<object?>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<object?>> RegisterAutoAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    }
}
