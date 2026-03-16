using SurveyBasket.Application.Services.Auth;
using SurveyBasket.Application.Services.Auth.Dtos;
using SurveyBasket.Application.Services.Auth.JWT;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IJWTProvider jWTProvider) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IJWTProvider _jWTProvider = jWTProvider;

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _authService.GetTokenAsync(request, cancellationToken);
            return StatusCode(response.Status, response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _jWTProvider.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return StatusCode(response.Status, response);
        }

        [HttpPut("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _jWTProvider.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return StatusCode(response.Status, response);
        }
    }
}
