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
using System.Text.Json;

namespace SurveyBasket.Infrastructure.Identity
{
    public class JWTProvider(IOptions<JwtOptions> jwtOptions ,
        SignInManager<ApplicationUser> signInManager
        , UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork) : IJWTProvider
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly int _refreshTokenExpiryDays = 30;

        public (string token, int expiresIn) GenerateToken(ApplicationUser user , IEnumerable<string>roles , IEnumerable<string> permissions)
        {
            Claim[] claims = [
                new (JwtRegisteredClaimNames.Sub, user.Id!),
                new (JwtRegisteredClaimNames.Email, user.Email!),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID used to -> Prevent Token Reuse
                new(nameof(roles),JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray),
                new(nameof(permissions),JsonSerializer.Serialize(permissions), JsonClaimValueTypes.JsonArray)
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

        public async Task<ApiResponse<object?>> GetRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            if (request.Token == null || request.RefreshToken == null)
            {
                messages.Add(new ApiResponseMessage("validation", "Token", "Token and Refresh Token are Required"));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status400BadRequest,
                       messages: messages);
            }

            var userId = ValidateToken(request.Token);

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

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == request.RefreshToken && x.IsActive);

            if(userRefreshToken == null )
            {
                messages.Add(new ApiResponseMessage("error", "RefreshToken", "The provided refresh token is invalid or has expired."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status401Unauthorized,
                    messages: messages
                );
            }

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _unitOfWork.UserRepository.GetAllPermissionsAsync(user,roles);

            var (newtoken, expiresIn) = GenerateToken(user, roles, permissions);

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

        public async Task<ApiResponse<object?>> RevokeRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            if (request.Token == null || request.RefreshToken == null)
            {
                messages.Add(new ApiResponseMessage("validation", "Token", "Token and Refresh Token are Required"));
                return new ApiResponse<object?>(
                       status: StatusCodes.Status400BadRequest,
                       messages: messages);
            }

            var userId = ValidateToken(request.Token);

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

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == request.RefreshToken && x.IsActive);

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
        public async Task<SignInResult> CheckUserSigninAsync(ApplicationUser user , string password)
        {
            return await _signInManager.PasswordSignInAsync(user, password,false,false);
        }
        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    }
}
