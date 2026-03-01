namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController : ControllerBase
    {
        private readonly IPollService _pollService;

        public PollsController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetList()
        {
            var polls  = await _pollService.GetList();

            if(polls.Status == 200) 
                return Ok(polls);
            return NotFound(polls);

        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var poll = await _pollService.GetById(id);

            if(poll.Status == 200) return Ok(poll);
            if(poll.Status == 404 ) return NotFound(poll);
            return BadRequest(poll);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(PollRequest request)
        {
            var poll = await _pollService.CreateAsync(request);

            if (poll.Status == 201) return CreatedAtAction("Create" , poll );

            return BadRequest(poll);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(int id , PollRequest request)
        {
            var poll = await _pollService.UpdateAsync(id , request);

            if (poll.Status == 200) return Ok(poll);
            if (poll.Status == 404) return NotFound(poll);
            return BadRequest(poll);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var poll = await _pollService.DeleteAsync(id);

            if (poll.Status == 200) return Ok(poll);
            if (poll.Status == 404) return NotFound(poll);
            return BadRequest(poll);
        }

        [HttpPut("{id}/TogglePublishStatus")]
        public async Task<IActionResult> TogglePublishStatus(int id)
        {
            var poll = await _pollService.TogglePublishStatusAsync(id);

            if (poll.Status == 200) return Ok(poll);
            if (poll.Status == 404) return NotFound(poll);
            return BadRequest(poll);
        }
    }
}
