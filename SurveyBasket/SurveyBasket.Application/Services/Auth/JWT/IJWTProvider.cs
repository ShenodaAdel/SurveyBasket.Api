using SurveyBasket.Application.Services.Auth.Dtos;
namespace SurveyBasket.Application.Services.Auth.JWT
{
    public interface IJWTProvider
    {
        ( string token , int expiresIn ) GenerateToken(TokenUserDto user);
    }
}
