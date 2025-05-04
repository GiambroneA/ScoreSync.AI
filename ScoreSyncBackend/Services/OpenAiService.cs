using System.Net.Http; // http requests to Groq
using System.Net.Http.Headers; // for the bearer token headers
using System.Text; // to encode the JSON request body
using System.Text.Json; // for parsing and generating JSON
using System.Threading.Tasks;
using ScoreSyncBackend.Models; // model for a team
using System.Collections.Generic; // for List<>

namespace ScoreSyncBackend.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient; // makes the request to Groq API
        private readonly IConfiguration _config; // reads API key from appsettings.json
        private readonly ScoreSyncDbContext _dbContext; // DB context to save team standings

        public OpenAIService(HttpClient httpClient, IConfiguration config, ScoreSyncDbContext dbContext)
        {
            _httpClient = httpClient;
            _config = config;
            _dbContext = dbContext;
        }

        public async Task<bool> UpdateStandingsFromOpenAIAsync(string matchResultsText)
        {
            // 🧠 prompt tells Groq to scrape and return table based on real standings
            string prompt = $@"
Using the current Premier League table from:
https://www.bbc.com/sport/football/premier-league/table

Return the full 2024–25 Premier League standings as a JSON array of exactly 20 teams, sorted by points (descending).
Each object in the array must have these fields:
- name (string)
- wins (int)
- draws (int)
- losses (int)
- goalsFor (int)
- goalsAgainst (int)

Only respond with the JSON array. No extra text.";

            // 🧱 the body of the POST request
            var requestBody = new
            {
                model = "llama3-70b-8192", // model Groq supports
                messages = new[]
                {
                    new { role = "system", content = "You are a football stats bot. Only respond with JSON arrays." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.2 // makes output more consistent
            };

            // 📦 convert request to JSON
            string jsonBody = JsonSerializer.Serialize(requestBody);

            // 📨 prepare the request to Groq endpoint
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["OpenAI:ApiKey"]);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            Console.WriteLine("📡 Sending request to Groq...");

            // 🌐 send the HTTP request
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Groq API call failed:");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return false;
            }

            // 📥 read the response body
            string responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("🔵 Raw Groq Response:");
            Console.WriteLine(responseString);

            // 🧱 extract only the message content
            string content;
            try
            {
                using var doc = JsonDocument.Parse(responseString);
                content = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .ToString()
                    .Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Failed to parse Groq response: " + ex.Message);
                return false;
            }

            Console.WriteLine("🟡 Cleaned content:");
            Console.WriteLine(content);

            // 🧼 isolate JSON part from full text (sometimes wrapped in ```)
            int firstBracket = content.IndexOf('[');
            int lastBracket = content.LastIndexOf(']');
            if (firstBracket == -1 || lastBracket == -1)
            {
                Console.WriteLine("⚠️ No JSON array detected");
                return false;
            }

            string cleanJson = content.Substring(firstBracket, lastBracket - firstBracket + 1);

            // 🧪 convert JSON string to a list of TeamStanding objects
            List<TeamStanding>? teams;
            try
            {
                teams = JsonSerializer.Deserialize<List<TeamStanding>>(cleanJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Failed to deserialize JSON to TeamStanding list:");
                Console.WriteLine("JSON:", cleanJson);
                Console.WriteLine("Error:", ex.Message);
                return false;
            }

            // 🚨 make sure we got valid data
            if (teams == null || teams.Count == 0)
            {
                Console.WriteLine("⚠️ Empty or invalid team list");
                return false;
            }

            // 🧮 calculate points and goal difference
            foreach (var team in teams)
            {
                team.GoalDifference = team.GoalsFor - team.GoalsAgainst;
                team.Points = (team.Wins * 3) + team.Draws;
            }

            // 💾 replace old data with new standings
            try
            {
                _dbContext.TeamStandings.RemoveRange(_dbContext.TeamStandings);
                _dbContext.TeamStandings.AddRange(teams);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ DB write failed: " + ex.Message);
                return false;
            }

            Console.WriteLine("✅ Standings updated successfully.");
            return true;
        }
    }
}
