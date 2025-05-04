using Microsoft.EntityFrameworkCore;
using ScoreSyncBackend.Models; //import TeamStanding class from ScoreSyncModels

namespace ScoreSyncBackend
{
    public class ScoreSyncDbContext : DbContext
    {
        public ScoreSyncDbContext(DbContextOptions<ScoreSyncDbContext> options) : base(options) { }

        public DbSet<TeamStanding> TeamStandings { get; set; }
        //does the creating of SQL table in database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<TeamStanding>()
        .Ignore(t => t.GoalDifference)
        .Ignore(t => t.Points);
}

    }
}
