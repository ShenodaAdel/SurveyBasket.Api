using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Auth.Filter;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("GetList")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> GetList()
        {
            var polls = await _pollService.GetList();

            if (polls.Status == StatusCodes.Status200OK)
                return Ok(polls);
            return NotFound(polls);
        }

        [HttpGet("GetCurrentList")]
        [Authorize(Roles = DefaultRoles.User)]
        public async Task<IActionResult> GetCurrentList()
        {
            var polls = await _pollService.GetCurrentList();

            if (polls.Status == StatusCodes.Status200OK)
                return Ok(polls);
            return NotFound(polls);
        }

        [HttpGet("GetById")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> GetById(int id)
        {
            var poll = await _pollService.GetById(id);

            if (poll.Status == StatusCodes.Status200OK) return Ok(poll);
            if (poll.Status == StatusCodes.Status404NotFound) return NotFound(poll);
            return BadRequest(poll);
        }

        [HttpPost("Create")]
        [HasPermission(Permissions.AddPolls)]
        public async Task<IActionResult> CreateAsync(PollRequest request)
        {
            var poll = await _pollService.CreateAsync(request);

            if (poll.Status == StatusCodes.Status201Created)
                return StatusCode(StatusCodes.Status201Created, poll);

            return BadRequest(poll);
        }

        [HttpPut("Update")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> UpdateAsync(int id, PollRequest request)
        {
            var poll = await _pollService.UpdateAsync(id, request);

            if (poll.Status == StatusCodes.Status200OK) return Ok(poll);
            if (poll.Status == StatusCodes.Status404NotFound) return NotFound(poll);
            return BadRequest(poll);
        }

        [HttpDelete("Delete")]
        [HasPermission(Permissions.DeletePolls)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var poll = await _pollService.DeleteAsync(id);

            if (poll.Status == StatusCodes.Status200OK) return Ok(poll);
            if (poll.Status == StatusCodes.Status404NotFound) return NotFound(poll);
            return BadRequest(poll);
        }

        [HttpPut("{id}/TogglePublishStatus")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> TogglePublishStatus(int id)
        {
            var poll = await _pollService.TogglePublishStatusAsync(id);

            if (poll.Status == StatusCodes.Status200OK) return Ok(poll);
            if (poll.Status == StatusCodes.Status404NotFound) return NotFound(poll);
            return BadRequest(poll);
        }
    }
}
