using Microsoft.AspNetCore.Authorization;
using SurveyBasket.API.Extensions;
using SurveyBasket.Application.Services.Users;
using SurveyBasket.Application.Services.Users.Dtos;

namespace SurveyBasket.API.Controllers
{
    [Route("me")]
    [ApiController]
    [Authorize]
    public class AccountController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await userService.GetUserProfileAsync(User.GetUserId()!);
            return StatusCode(result.Status, result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var result = await userService.UpdateUserProfileAsync(User.GetUserId()!, request);
            return StatusCode(result.Status, result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await userService.ChangePasswordAsync(User.GetUserId()!, request);
            return StatusCode(result.Status, result);
        }
    }
}
