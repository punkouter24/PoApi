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
    }

}



