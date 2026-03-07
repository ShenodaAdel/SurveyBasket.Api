using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Application.Services.Auth.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Infrastructure.Identity
{
    public class JWTProvider : IJWTProvider
    {
        public (string token, int expiresIn) GenerateToken(TokenUserDto user)
        {
            Claim[] claims = [
                new (JwtRegisteredClaimNames.Sub, user.Id!),
                new (JwtRegisteredClaimNames.Email, user.Email!),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ];
             
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes());

            var jwt = new JwtSecurityTokenHandler(
        }
    }
}
