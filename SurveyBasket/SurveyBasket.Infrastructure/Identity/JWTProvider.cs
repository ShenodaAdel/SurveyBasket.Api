using Microsoft.AspNetCore.Http;
using SurveyBasket.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Application.Services.Auth.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Infrastructure.Identity
{
    public class JWTProvider(IOptions<JwtOptions> jwtOptions , UserManager<ApplicationUser> userManager) : IJWTProvider
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly int _refreshTokenExpiryDays = 30;

        public (string token, int expiresIn) GenerateToken(TokenUserDto user)
        {
            Claim[] claims = [
                new (JwtRegisteredClaimNames.Sub, user.Id!),
                new (JwtRegisteredClaimNames.Email, user.Email!),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID used to -> Prevent Token Reuse
                ]; // some of cliams you want to add it in token 

            // Link web site i used it to catch Secret key by ssh 256 -> https://acte.ltd/utils/randomkeygen?utm_source=chatgpt.com
            // responsable to Encoding and Decoding
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)); 

            // Secret Ket + Algorthims 
            var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var expiresIn = _jwtOptions.ExpiryMinutes;

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer, // who create Token 
                audience : _jwtOptions.Audience,
                claims : claims,
                expires : DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials : singingCredentials 
                ); // Shape of Token 

            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn * 60 );
        }

        public string? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            }
            catch
            {
                return null;
            }
        }

        public async Task<ApiResponse<object?>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            if (token == null || refreshToken == null)
            {
                messages.Add(new ApiResponseMessage("validation", "Token", "Token and Refresh Token are Required"));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status400BadRequest,
                       messages: messages);
            }

            var userId = ValidateToken(token);

            if (userId == null)
            {
                messages.Add(new ApiResponseMessage("error", "Token", "Invalid or expired access token."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status401Unauthorized,
                    messages: messages
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            
            if(user == null)
            {
                messages.Add(new ApiResponseMessage("error", "User", "User associated with the provided token does not exist."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages
                );
            }

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if(userRefreshToken == null )
            {
                messages.Add(new ApiResponseMessage("error", "RefreshToken", "The provided refresh token is invalid or has expired."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status401Unauthorized,
                    messages: messages
                );
            }

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var tokenUser = new TokenUserDto { Email = user.Email , Id = user.Id };

            var (newtoken, expiresIn) = GenerateToken(tokenUser);

            var newRefreshToken = GenerateRefreshToken();

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = refreshTokenExpiration,
            });

            await _userManager.UpdateAsync(user);

            var authResponse = new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = newtoken,
                ExpiresIn = expiresIn,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiration = refreshTokenExpiration

            };

            messages.Add(new ApiResponseMessage("success", "Authentication", "Access token and refresh token have been successfully renewed."));
            return new ApiResponse<object?>(
                data: authResponse,
                status: StatusCodes.Status200OK,
                messages: messages
            );

        }

        public async Task<ApiResponse<object?>> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            if (token == null || refreshToken == null)
            {
                messages.Add(new ApiResponseMessage("validation", "Token", "Token and Refresh Token are Required"));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status400BadRequest,
                       messages: messages);
            }

            var userId = ValidateToken(token);

            if (userId == null)
            {
                messages.Add(new ApiResponseMessage("error", "Token", "Invalid or expired access token."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status401Unauthorized,
                    messages: messages
                );
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                messages.Add(new ApiResponseMessage("error", "User", "User associated with the provided token does not exist."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages
                );
            }

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (userRefreshToken == null)
            {
                messages.Add(new ApiResponseMessage("error", "RefreshToken", "The provided refresh token is invalid or has expired."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status401Unauthorized,
                    messages: messages
                );
            }

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            messages.Add(new ApiResponseMessage("success", "Authentication", "Refresh token has been successfully revoked."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages
            );

        }
        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    }
}
