using SurveyBasket.Application.Services.Auth.Dtos;
namespace SurveyBasket.Application.Services.Auth.JWT
{
    public interface IJWTProvider
    {
        ( string token , int expiresIn ) GenerateToken(TokenUserDto user);

        string? ValidateToken(string  token);
        Task<ApiResponse<object?>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<ApiResponse<object?>> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    }
}
