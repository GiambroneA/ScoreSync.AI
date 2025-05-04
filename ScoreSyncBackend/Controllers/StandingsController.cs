using Microsoft.AspNetCore.Mvc; // Gives access to web-related tools like API controllers, HTTP methods (GET, POST)
using ScoreSyncBackend.Models; // Allows use of TeamStanding model
using ScoreSyncBackend.Services; // Gives access to OpenAIService

namespace ScoreSyncBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Sets route to /api/standings
    public class StandingsController : ControllerBase
    {
        private readonly ScoreSyncDbContext _context;          // Database context for querying and updating standings
        private readonly OpenAIService _openAIService;         // OpenAI service to call AI for standings

        // Constructor to inject dependencies
        public StandingsController(ScoreSyncDbContext context, OpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        // GET: api/standings
        [HttpGet]
        public IActionResult GetStandings()
        {
            var standings = _context.TeamStandings.ToList(); // Fetch all team standings from DB
            return Ok(standings); // Return them as JSON
        }

        // POST: api/standings
        [HttpPost]
        public IActionResult AddTeam([FromBody] TeamStanding newTeam)
        {
            _context.TeamStandings.Add(newTeam); // Add new team to DB
            _context.SaveChanges(); // Commit change
            return CreatedAtAction(nameof(GetStandings), new { id = newTeam.TeamID }, newTeam); // Return created response
        }

        // ✅ NOTE:
        // The real OpenAI call should happen in OpenAiController.cs
        // That controller accepts match results input from the frontend,
        // and correctly passes it into OpenAIService.UpdateStandingsFromOpenAIAsync(string matchResultsText)
    }
}
