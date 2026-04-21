using Microsoft.AspNetCore.Identity;
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Domain.Entities;

namespace SurveyBasket.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AuthResponse?> ValidateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return null;

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!isValidPassword)
                return null;

            return new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<ApplicationUser?> GetUserByEmaiAndPasswordlAsync(string email , string password)
        {
            var user =  await _userManager.FindByEmailAsync(email);
            return user is not null && await _userManager.CheckPasswordAsync(user, password) ? user : null;
        }

        public void AddRefreshToken(ApplicationUser user, string token, DateTime refreshTokenExpiration)
        {
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = token,
                ExpiresOn = refreshTokenExpiration
            });
        }

        public async Task UpdateUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return;

            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> CheckExistUser(string email)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<IdentityResult> CreateUserByPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
                return await _userManager.ConfirmEmailAsync(user, token);
        }
    }
}
