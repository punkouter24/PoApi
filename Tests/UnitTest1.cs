using Microsoft.Extensions.Configuration;
using PoApi.Models;

namespace Tests
{
    public class HighScoreServiceTests
    {
        private readonly HighScoreService _service;

        public HighScoreServiceTests()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _service = new HighScoreService(configuration);
        }

        [Fact]
        public async Task AddHighScoreAsync_AddsScoreSuccessfully()
        {
            // Arrange
            HighScore highScore = new()
            {
                PartitionKey = "TestGame",
                RowKey = Guid.NewGuid().ToString(), // Ensure unique
                Name = "Test User",
                Score = 100.0,
                AppName = "TestGame"
            };

            // Act
            await _service.AddHighScoreAsync(highScore);

            // Assert
            HighScore? insertedScore = (await _service.GetTopScoresAsync()).Find(hs => hs.RowKey == highScore.RowKey);
            Assert.NotNull(insertedScore);

            // Cleanup
           // await _service.DeleteHighScoreAsync(highScore);
        }

        [Fact]
        public async Task GetTopScoresAsync_ReturnsTop10Scores()
        {
            // Arrange
            var fakeScores = Enumerable.Range(1, 15).Select(i => new HighScore
            {
                PartitionKey = "TestGame",
                RowKey = Guid.NewGuid().ToString(),
                Name = $"Test User {i}",
                Score = 1 + i, // Ensuring unique and sortable scores and smaller than anything that exists
                AppName = "TestGame"
            }).ToList();

            foreach (var score in fakeScores)
            {
                await _service.AddHighScoreAsync(score);
            }

            // Act
            var topScores = await _service.GetTopScoresAsync();

            // Assert
            Assert.NotNull(topScores);
            Assert.Equal(10, topScores.Count); // Only top 10 scores should be returned
            var expectedTopScores = fakeScores.OrderBy(s => s.Score).Take(10).ToList();
            for (int i = 0; i < expectedTopScores.Count; i++)
            {
                Assert.Equal(expectedTopScores[i].RowKey, topScores[i].RowKey); // Verify each score is in the correct order
            }

            // Cleanup
            foreach (var score in fakeScores)
            {
                await _service.DeleteHighScoreAsync(score);
            }
        }
    }

}



