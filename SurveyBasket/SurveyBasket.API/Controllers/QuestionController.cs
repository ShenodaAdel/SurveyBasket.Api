using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Application.Services.Question;
using SurveyBasket.Application.Services.Question.Dtos;

namespace SurveyBasket.API.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    //[Authorize]
    public class QuestionController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;
        [HttpPost("")]
        public async Task<IActionResult> CreateAsync([FromRoute] int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _questionService.CreateAsync(pollId, request, cancellationToken);
            if (result.Status == 201)
                return Created(nameof(CreateAsync),result);
            else if (result.Status == 404)
                return NotFound(result);
            return BadRequest(result);
        }
    }
}
