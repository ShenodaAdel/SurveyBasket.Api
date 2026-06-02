using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Auth.Filter;
using SurveyBasket.Application.Services.Users;
using SurveyBasket.Application.Services.Users.Dtos;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            return StatusCode(result.Status, result);
        }

        [HttpGet("GetById/{id}")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var result = await _userService.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPost("")]
        [HasPermission(Permissions.AddUsers)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateAsync(id, request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}/toggle-status")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> ToggleUser([FromRoute] string id)
        {
            var result = await _userService.ToggleAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}/unlock")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> UnLockAsync([FromRoute] string id)
        {
            var result = await _userService.UnLockAsync(id);
            return StatusCode(result.Status, result);
        }
    }
}
