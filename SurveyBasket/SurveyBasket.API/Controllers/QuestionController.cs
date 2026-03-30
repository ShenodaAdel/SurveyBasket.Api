using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Application.Services.Question;
using SurveyBasket.Application.Services.Question.Dtos;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class QuestionController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;
        [HttpPost]
        public async Task<IActionResult> CreateAsync( int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _questionService.CreateAsync(pollId, request, cancellationToken);
            if (result.Status == 201)
                return Created(nameof(CreateAsync),result);
            else if (result.Status == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetListByPollId(int pollId)
        {
            var result = await _questionService.GetListByPollId(pollId);
            if(result.Status == 200)
                return Ok(result);
            return NotFound(result);
        }
        [HttpGet("GetByPollId")]
        public async Task<IActionResult> GetByPollId(int pollId , int id)
        {
            var result = await _questionService.GetByPollId(pollId , id);
            if (result.Status == 200)
                return Ok(result);
            return NotFound(result);
        }
        [HttpPut("ToggleStatus")]
        public async Task<IActionResult> TogglePublishStatus(int pollId , int id)
        {
            var result = await _questionService.ToggleStatusAsync(pollId , id);

            if (result.Status == StatusCodes.Status200OK) return Ok(result);
            if (result.Status == StatusCodes.Status404NotFound) return NotFound(result);
            return BadRequest(result);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(int pollId, int id , [FromBody]  QuestionRequest request)
        {
            var result = await _questionService.UpdateAsync(pollId, id, request);

            if (result.Status == StatusCodes.Status200OK) return Ok(result);
            if (result.Status == StatusCodes.Status404NotFound) return NotFound(result);
            return BadRequest(result);      
        }
    }
}
