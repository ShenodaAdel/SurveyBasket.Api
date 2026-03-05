using SurveyBasket.Application.Services.Auth.Dtos;

namespace SurveyBasket.Application.RepositoriesInterfaces
{
    public interface IUserRepository
    {
        Task<AuthResponse?> ValidateUserAsync( string email , string password );
    }
}
