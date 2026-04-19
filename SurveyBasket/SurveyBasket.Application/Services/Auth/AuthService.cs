using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Application.Services.Auth.JWT;
using SurveyBasket.Domain.Entities;
using System.Security.Cryptography;

namespace SurveyBasket.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<LoginRequest> _validator;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IJWTProvider _jWTProvider;
        private readonly int _refreshTokenExpiryDays = 30;

        public AuthService(IUnitOfWork unitOfWork, IValidator<LoginRequest> validator, 
            IValidator<RegisterRequest> registerValidator, IJWTProvider jWTProvider)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _jWTProvider = jWTProvider;
            _registerValidator = registerValidator;
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

            var user = await _unitOfWork.UserRepository.ValidateUserAsync(request.Email, request.Password);

            if (user == null)
            {
                messages.Add(new ApiResponseMessage("error", "Authentication", "Invalid email or password."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status401Unauthorized,
                    messages: messages);
            }

            var tokenUser = new TokenUserDto
            {
                Email = user.Email,
                Id = user.Id
            };

            var (token, expiresIn) = _jWTProvider.GenerateToken(tokenUser);

            var refreshToken = GenerateRefreshToken();

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            await _unitOfWork.UserRepository.AddRefreshToken(user.Email!, refreshToken, refreshTokenExpiration);
            await _unitOfWork.UserRepository.UpdateUser(user.Email!);
            await _unitOfWork.SaveChangesAsync();

            var authResponse = new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                ExpiresIn = expiresIn,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            messages.Add(new ApiResponseMessage("success", "Authentication", "User authenticated successfully."));
            return new ApiResponse<object?>(
                data: authResponse,
                status: StatusCodes.Status200OK,
                messages: messages
            );
        }


        public async Task<ApiResponse<object?>> RegisterAutoAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();
            var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
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

            var existingUser = await _unitOfWork.UserRepository.CheckExistUser(request.Email,cancellationToken);
            if (existingUser)
            {
                messages.Add(new ApiResponseMessage("error", "Registration", "Email is already in use."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await _unitOfWork.UserRepository.CreateUserByPasswordAsync(user, request.Password);

            if(!result.Succeeded)
            {
                messages.AddRange(result.Errors.Select(error =>
                    new ApiResponseMessage("error", "Registration", error.Description)
                ));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages
                );
            }

            var tokenUser = new TokenUserDto
            {
                Email = user.Email,
                Id = user.Id
            };

            var (token, expiresIn) = _jWTProvider.GenerateToken(tokenUser);

            var refreshToken = GenerateRefreshToken();

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            await _unitOfWork.UserRepository.AddRefreshToken(user.Email!, refreshToken, refreshTokenExpiration);
            await _unitOfWork.SaveChangesAsync();

            var authResponse = new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                ExpiresIn = expiresIn,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            messages.Add(new ApiResponseMessage("success", "Registration", "User registered successfully."));
            return new ApiResponse<object?>(
                data: authResponse,
                status: StatusCodes.Status200OK,
                messages: messages
            );
        }


        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
