using Microsoft.AspNetCore.Mvc;
using ScoreSyncBackend.Services;
using System.Threading.Tasks;

namespace ScoreSyncBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenAiController : ControllerBase
    {
        private readonly OpenAIService _openAiService;

        public OpenAiController(OpenAIService openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpPost("standings")]
        public async Task<IActionResult> UpdateStandings([FromBody] MatchResultsRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Result))
                return BadRequest("Match results input is required.");

            var success = await _openAiService.UpdateStandingsFromOpenAIAsync(request.Result);

            if (!success)
                return StatusCode(500, "Failed to update standings.");

            return Ok("Standings updated successfully.");
        }
    }

    public class MatchResultsRequest
    {
        public string Result { get; set; } = string.Empty;
    }
}
