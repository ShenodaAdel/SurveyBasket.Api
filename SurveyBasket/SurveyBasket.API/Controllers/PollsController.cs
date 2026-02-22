using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPolls()
        {
            // Logic to retrieve polls from the database
            return Ok(new { Message = "List of polls" });
        }

    }
}
