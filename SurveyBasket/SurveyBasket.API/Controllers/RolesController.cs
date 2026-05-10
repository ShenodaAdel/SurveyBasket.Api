using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Auth.Filter;
using SurveyBasket.Application.Services.Role;
using SurveyBasket.Application.Services.Role.Dtos;
using SurveyBasket.Infrastructure.Repositories;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService roleService = roleService;

        [HttpGet("GetAll")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetAll([FromQuery] bool? includeDisabled = false)
        {
            var result = await roleService.GetAllAsync(includeDisabled);
            return StatusCode(result.Status, result);
        }

        [HttpGet("GetDetailById/{id}")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetDetailById([FromRoute] string id)
        {
            var result = await roleService.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }
            
        [HttpPost("CreateRole")]
        [HasPermission(Permissions.AddRoles)]
        public async Task<IActionResult> CreateAsync([FromBody] RoleRequest request)
        {
            var result = await roleService.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("UpdateRole/{id}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] RoleRequest request)
        {
            var result = await roleService.UpdateAsync(id, request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("ToggleRole/{id}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> ToggleAsync([FromRoute] string id)
        {
            var result = await roleService.ToggleAsync(id);
            return StatusCode(result.Status, result);
        }
    }
}
