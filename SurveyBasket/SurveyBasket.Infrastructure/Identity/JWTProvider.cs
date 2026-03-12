using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Application.Services.Auth.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Infrastructure.Identity
{
    public class JWTProvider(IOptions<JwtOptions> jwtOptions) : IJWTProvider
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

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
    }
}
