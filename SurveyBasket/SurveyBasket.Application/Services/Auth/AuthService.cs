using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Application.Services.Auth.JWT;
using SurveyBasket.Application.Services.Email;
using SurveyBasket.Domain.Entities;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SurveyBasket.Application.Services.Auth
{
    public class AuthService(IUnitOfWork unitOfWork, IEmailService emailService,
        IJWTProvider jWTProvider, ILogger<AuthService> logger,
        UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IJWTProvider _jWTProvider = jWTProvider;
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly int _refreshTokenExpiryDays = 30;


        // Login 
        public async Task<ApiResponse<object?>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            if (await _unitOfWork.UserRepository.GetUserByEmaiAndPasswordlAsync(request.Email, request.Password) is not { } user) // is not object
            {
                messages.Add(new ApiResponseMessage("error", "Authentication", "Invalid email or password."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status401Unauthorized,
                    messages: messages);
            }



            if (user.EmailConfirmed)
            {
                var tokenUser = new TokenUserDto
                {
                    Email = user.Email,
                    Id = user.Id
                };

                var (token, expiresIn) = _jWTProvider.GenerateToken(tokenUser);

                var refreshToken = GenerateRefreshToken();

                var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

                _unitOfWork.UserRepository.AddRefreshToken(user!, refreshToken, refreshTokenExpiration);
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
            else
            {
                messages.Add(new ApiResponseMessage("error", "Authentication", "User is not allowed to sign in."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status403Forbidden,
                    messages: messages);
            }
        }

        public async Task<ApiResponse<object?>> RegisterAutoAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var messages = new List<ApiResponseMessage>();

            var existingUser = await _unitOfWork.UserRepository.CheckExistUser(request.Email);
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

            if (!result.Succeeded)
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

            _unitOfWork.UserRepository.AddRefreshToken(user!, refreshToken, refreshTokenExpiration);
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

        public async Task<ApiResponse<object?>> RegisterAsync(RegisterRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            var existingUser = await _unitOfWork.UserRepository.CheckExistUser(request.Email);
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

            if (!result.Succeeded)
            {
                messages.AddRange(result.Errors.Select(error =>
                    new ApiResponseMessage("error", "Registration", error.Description)
                ));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages
                );
            }

            var code = await _unitOfWork.UserRepository.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("Email confirmation code for user {Email}: {Code}", user.Email, code);

            await SendConfirmationEmail(user, code);

            messages.Add(new ApiResponseMessage("success", "Registration", "User registered successfully. Please check your email for the confirmation code."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages
            );

        }

        public async Task<ApiResponse<object?>> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            if (await _unitOfWork.UserRepository.GetUserByIdAsync(request.UserId) is not { } user)
            {
                messages.Add(new ApiResponseMessage("error", "Email Confirmation", "Code is not correct"));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status404NotFound,
                    messages: messages);
            }

            if (user.EmailConfirmed)
            {
                messages.Add(new ApiResponseMessage("error", "Email Confirmation", "Email is already confirmed."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var code = request.Code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                messages.Add(new ApiResponseMessage("error", "Email Confirmation", "Invalid confirmation code format."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }



            var result = await _unitOfWork.UserRepository.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                messages.AddRange(result.Errors.Select(error =>
                    new ApiResponseMessage("error", "Email Confirmation", error.Description)
                ));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages
                );
            }

            messages.Add(new ApiResponseMessage("success", "Email Confirmation", "Email confirmed successfully. You can now log in."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages
            );

        }

        public async Task<ApiResponse<object?>> ResendConfirmationEmailAsync(ResendConfirmationEmail request)
        {
            var messages = new List<ApiResponseMessage>();

            if (await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email) is not { } user)
            {
                messages.Add(new ApiResponseMessage("success", "Resend Confirmation Email", "Resend Operation Email Success."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status200OK,
                    messages: messages);
            }

            if (user.EmailConfirmed)
            {
                messages.Add(new ApiResponseMessage("Failer", "Resend Confirmation Email", "This email is aleardy confirmed."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var code = await _unitOfWork.UserRepository.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await SendConfirmationEmail(user, code);

            messages.Add(new ApiResponseMessage("success", "Resend Confirmation Email", "Resend Operation Email Success. Please check your email for the confirmation code."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages);

        }

        public async Task<ApiResponse<object?>> SendResetPasswordCodeAsync(ForgetPasswordRequest request)
        {
            var messages = new List<ApiResponseMessage>();

            if (await _userManager.FindByEmailAsync(request.Email) is not { } user || !user.EmailConfirmed)
            { 
                messages.Add(new ApiResponseMessage("Success", "Reset Password", "Check Your Email!"));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status200OK,
                    messages: messages);
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("Reset Code : {Code}", code);

            await SendResetPasswordEmail(user, code);

            messages.Add(new ApiResponseMessage("success", "Reset Password", "A password reset link has been sent.+ Please check your email."));
            return new ApiResponse<object?>(
                status: StatusCodes.
                Status200OK,
                messages: messages);


        }

        public async Task<ApiResponse<object?>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var messages = new List<ApiResponseMessage>();
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user || !user.EmailConfirmed)
            {
                messages.Add(new ApiResponseMessage("Error", "Reset Password", "Code is Invalid"));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var code = request.Code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                messages.Add(new ApiResponseMessage("error", "Reset Password", "Invalid reset code format."));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages);
            }

            var result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            if (!result.Succeeded)
            {
                messages.AddRange(result.Errors.Select(error =>
                    new ApiResponseMessage("error", "Reset Password", error.Description)
                ));
                return new ApiResponse<object?>(
                    status: StatusCodes.Status400BadRequest,
                    messages: messages
                );
            }

            messages.Add(new ApiResponseMessage("success", "Reset Password", "Password reset successful."));
            return new ApiResponse<object?>(
                status: StatusCodes.Status200OK,
                messages: messages
            );
        }

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private Task SendResetPasswordEmail(ApplicationUser user, string code)
        {
            // the frontend must send me a this link 
            var confirmUrl = _httpContextAccessor.HttpContext!.Request.Headers["Origin"].ToString();

            var emailBody = EmailBodyBuilder.BuildEmailConfirmationBody("ForgetPassword",
                new Dictionary<string, string>
                {
                    { "{name}", user.FirstName },
                    { "{url}", $"{confirmUrl}/auth/forgetPassword?email={user.Email}&code={code}" }
                });

            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email!, "✔ Survey Basket: Change Password", emailBody));

            return Task.CompletedTask;
        }

        private async Task SendConfirmationEmail(ApplicationUser user, string code)
        {
            // the frontend must send me a this link 
            var confirmUrl = $"http://localhost:5173/confirm-email?userId={user.Id}&code={code}";

            var emailBody = EmailBodyBuilder.BuildEmailConfirmationBody("EmailConfirmation",
                new Dictionary<string, string>
                {
                    { "{name}", user.FirstName },
                    { "{url}", confirmUrl }
                });

            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody));

            await Task.CompletedTask;
        }

    }
}
