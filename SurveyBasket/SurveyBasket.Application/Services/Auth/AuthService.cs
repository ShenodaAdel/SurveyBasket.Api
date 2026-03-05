
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Domain.Entities;

namespace SurveyBasket.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<object?>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            // validation
            if (string.IsNullOrWhiteSpace(email))
            {
                messages.Add(new ApiResponseMessage("validation", "Email", "Email is required."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                messages.Add(new ApiResponseMessage("validation", "Password", "Password is required."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }


            var user =  await _unitOfWork.UserRepository.ValidateUserAsync(email,password);

            if (user == null) 
            {
                messages.Add(new ApiResponseMessage("error", "Authentication", "Invalid email or password."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status401Unauthorized,
                    messages: messages);
            }



            // generate jwt token 

            var authResponse = new AuthResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LirstName,
                null,
                0
            );

            messages.Add(new ApiResponseMessage("success", "Authentication", "User authenticated successfully."));
            return new ApiResponse<object?>(
                data: authResponse,
                status: StatusCodes.Status200OK,
                messages: messages
            );
        }
    }
}
