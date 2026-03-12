using SurveyBasket.Application.Services.Auth;
using SurveyBasket.Application.Services.Auth.Dtos;
namespace SurveyBasket.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService , IConfiguration configuration) : ControllerBase
    {
        private readonly IAuthService authService = authService;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("")]
        public async Task<IActionResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var response = await authService.GetTokenAsync(request, cancellationToken);

            return StatusCode(response.Status, response);

        }

    }
}
