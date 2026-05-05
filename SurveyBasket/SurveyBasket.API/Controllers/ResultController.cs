using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Application.Helpers;
using SurveyBasket.Application.Services.Auth.Filter;
using SurveyBasket.Application.Services.Result;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [HasPermission(Permissions.Results)]
    public class ResultController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        [HttpGet("{pollId}")]
        public async Task<IActionResult> GetPollVotes([FromRoute]int pollId , CancellationToken cancellationToken = default)
        {
            var result = await _resultService.GetPollVotesAsync(pollId, cancellationToken);
            if(result.Status == 200) return Ok(result);
            else if (result.Status == 404) return NotFound(result);
            return BadRequest(result);
        }

        [HttpGet("{pollId}/votes-per-day")]
        public async Task<IActionResult> GetVotesPerDay([FromRoute]int pollId , CancellationToken cancellationToken = default)
        {
            var result = await _resultService.GetVotesPerDayAsync(pollId, cancellationToken);
            if(result.Status == 200) return Ok(result);
            else if (result.Status == 404) return NotFound(result);
            return BadRequest(result);
        }

        [HttpGet("{pollId}/votes-per-question")]
        public async Task<IActionResult> GetVotesPerQuestion([FromRoute]int pollId , CancellationToken cancellationToken = default)
        {
            var result = await _resultService.GetVotesPerQuestionAsync(pollId, cancellationToken);
            if(result.Status == 200) return Ok(result);
            else if (result.Status == 404) return NotFound(result);
            return BadRequest(result);
        }
    }
}
