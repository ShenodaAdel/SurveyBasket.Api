using Microsoft.AspNetCore.Identity;
using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IUserRepository
    {
        Task<AuthResponse?> ValidateUserAsync( string email , string password );
        Task AddRefreshToken(string email, string token, DateTime refreshTokenExpiration);
        Task UpdateUser(string email);
        Task<bool> CheckExistUser(string email, CancellationToken cancellationToken = default);
        Task<IdentityResult> CreateUserByPasswordAsync(ApplicationUser user, string password);
    }
}
