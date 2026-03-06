
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Domain.Entities;

namespace SurveyBasket.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<LoginRequest> _validator;

        public AuthService(IUnitOfWork unitOfWork, IValidator<LoginRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<ApiResponse<object?>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                messages.AddRange(validationResult.Errors.Select(error =>
                    new ApiResponseMessage("validation", error.PropertyName, error.ErrorMessage)
                ));

                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages
                );
            }


            var user =  await _unitOfWork.UserRepository.ValidateUserAsync(request.Email, request.Password);

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
