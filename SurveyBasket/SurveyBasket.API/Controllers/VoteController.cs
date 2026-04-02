using Microsoft.AspNetCore.Authorization;
using SurveyBasket.API.Extensions;
using SurveyBasket.Application.Services.Question;
using SurveyBasket.Application.Services.Vote;
using SurveyBasket.Application.Services.Vote.Dtos;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VoteController(IQuestionService questionService , IVoteService voteService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;
        private readonly IVoteService _voteService = voteService;

        [HttpGet("GetListAvaibale/{pollid}")]
        public async Task<IActionResult> GetAvaibalePoll([FromRoute]int pollId)
        {
            var userId = User.GetUserId();
            var result = await _questionService.GetAvailableListByPollId(pollId, userId!);

            if(result.Status == 200) return Ok(result);
            else if (result.Status == 404) return NotFound(result);
            return BadRequest(result);

        }
        [HttpPost("{pollId}")]
        public async Task<IActionResult> Create([FromRoute] int pollId , VoteRequest request)
        {
            var userId = User.GetUserId();
            var result = await _voteService.CreateAsync(pollId, userId!,request);
            if( result.Status == 201) return CreatedAtAction(nameof(Create), result);
            else if ( result.Status == 404) return NotFound(result);
                return BadRequest(result);
            
        }
    }
}
