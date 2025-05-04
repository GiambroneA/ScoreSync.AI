# ScoreSync.AI

ScoreSync.AI is a full-stack web application that displays live English Premier League standings using data generated via Grok AI’s Open API.

## 🛠 Tech Stack

- HTML, CSS, JavaScript (Frontend)
- C# with ASP.NET Core (Backend)
- SQLite with Entity Framework Core (Database)
- Grok AI API (for generating live football table data)

## 📦 Features

- Live updating Premier League table
- "Update Standings" button pulls latest results via Grok AI
- Data stored and served from a local SQLite database
- Clean, responsive table layout styled with CSS

## ⚠️ Known Limitations

- Grok AI cannot scrape live web data, so standings may not be fully accurate
- Requires manual prompt tuning for reliable output formatting

## 🚧 Challenges Faced

- Integrating Grok’s AI responses and parsing the correct JSON structure
- Handling API limits and fallback handling
- Grok lacks browsing/search capabilities, so the AI could not return the true Premier League table


## ✅ How to Run

1. Clone this repo  
2. Run `dotnet build` and `dotnet run` from the backend folder  
3. Open `index.html` in your browser  
4. Click the "Update Standings" button to load data from the API

---

Feel free to reach out if you'd like to build your own version for another sport!
