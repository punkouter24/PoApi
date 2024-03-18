using PoApi.Models;
using Microsoft.Extensions.Azure;
using Azure.Identity;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Make sure Configuration is available for DI
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IHighScoreService, HighScoreService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
//}

app.UseHttpsRedirection();



// Endpoint to add a high score
app.MapPost("/highscores", async (HighScore highScore, IHighScoreService highScoreService) =>
{
   // PartitionKey = highScoreRequest.AppName, // Example: Use AppName as PartitionKey
   //     RowKey = Guid.NewGuid().ToString(), // Generate a unique RowKey

    highScore.PartitionKey = highScore.AppName;
    highScore.RowKey = Guid.NewGuid().ToString();
    await highScoreService.AddHighScoreAsync(highScore);
    return Results.Ok(highScore);
});

// Endpoint to get top 10 high scores
app.MapGet("/highscores/top", async (IHighScoreService highScoreService) =>
{
    List<HighScore> topScores = await highScoreService.GetTopScoresAsync();
    return Results.Ok(topScores);
});



string[] summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    WeatherForecast[] forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
