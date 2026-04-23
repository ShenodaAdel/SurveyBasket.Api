using Microsoft.AspNetCore.Identity;
using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IUserRepository
    {
        Task<AuthResponse?> ValidateUserAsync( string email , string password );
        void AddRefreshToken(ApplicationUser user, string token, DateTime refreshTokenExpiration);
        Task UpdateUser(string email);
        Task<bool> CheckExistUser(string email);
        Task<IdentityResult> CreateUserByPasswordAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> GetUserByEmaiAndPasswordlAsync(string email, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
    }
}
