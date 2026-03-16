using Microsoft.AspNetCore.Identity;
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Infrastructure.Identity;

namespace SurveyBasket.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository 
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Create Functions 

        public async Task<AuthResponse?> ValidateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!isValidPassword)
                return null;

            return new AuthResponse {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName 
            };
        }

        public async Task AddRefreshToken ( string email , string token , DateTime refreshTokenExpiration)
        {
            var user = await _userManager.FindByEmailAsync(email)!;

            user!.RefreshTokens.Add(new RefreshToken
            {
                Token = token,
                ExpiresOn = refreshTokenExpiration
            });
            
        }

        public async Task UpdateUser (string email)
        {
            var user = await _userManager.FindByEmailAsync(email)!;

            await _userManager.UpdateAsync(user!);
        }
    }
}
