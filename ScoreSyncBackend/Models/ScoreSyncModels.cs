using System.ComponentModel.DataAnnotations;
namespace ScoreSyncBackend.Models

{
    // Represents a team and its current standing in the Premier League
    public class TeamStanding
    {
        [Key] // tells EF core this is a primary key 
        public int TeamID { get; set; }  // Unique ID for the team
        public string Name { get; set; } = string.Empty;  // Name of the team (e.g., Arsenal)
        public int Wins { get; set; }  // Total wins
        public int Draws { get; set; }  // Total draws
        public int Losses { get; set; }  // Total losses
        public int GoalsFor { get; set; }  // Goals scored
        public int GoalsAgainst { get; set; }  // Goals conceded

        public int GoalDifference { get; set; }  // Writable instead of calculated
        public int Points { get; set; }  // Writable instead of calculated
    }
}

/*
 * This is a data modeling file. It defines the shape and structure of your app’s data.
 * Each instance of this class represents one row in the standings table.
 * It lets us work with data using C# classes instead of raw SQL.
 *
 * How this file interacts with the rest of the app:
 *
 * ▸ Database (SQLite): EF Core uses this to create a "TeamStandings" table automatically.
 * ▸ DbContext: Lets us run queries like dbContext.TeamStandings.ToList() or Add(newTeam).
 * ▸ API Controller: This is the object your API returns to the frontend as JSON.
 * ▸ Frontend (HTML/JS): JavaScript will receive objects like { name: "Arsenal", wins: 12, ... } — matching this structure.
 * ▸ OpenAI Logic: When OpenAI generates match or team data, it will map into this structure before saving to the database.
 */
