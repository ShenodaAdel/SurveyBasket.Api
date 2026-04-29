using Microsoft.AspNetCore.Identity;
using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.Services.Auth.JWT
{
    public interface IJWTProvider
    {
        ( string token , int expiresIn ) GenerateToken(TokenUserDto user);

        string? ValidateToken(string  token);
        Task<ApiResponse<object?>> GetRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<object?>> RevokeRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task<SignInResult> CheckUserSigninAsync(ApplicationUser user, string password);
    }
}
