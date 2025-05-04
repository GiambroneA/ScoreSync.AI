using Microsoft.EntityFrameworkCore;
using ScoreSyncBackend;
using ScoreSyncBackend.Services; 

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with SQLite
builder.Services.AddDbContext<ScoreSyncDbContext>(options =>
    options.UseSqlite("Data Source=scoresync.db"));

// Add support for controllers (API endpoints)
builder.Services.AddControllers();

// Register custom OpenAIService for AI access
builder.Services.AddHttpClient<OpenAIService>();
builder.Services.AddScoped<OpenAIService>();

//  Add CORS policy to allow any origin (needed for frontend to call backend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ✅ Use the CORS policy BEFORE routing
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
