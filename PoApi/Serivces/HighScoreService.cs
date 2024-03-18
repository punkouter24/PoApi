using Microsoft.Azure.Cosmos.Table;
using PoApi.Models;

public interface IHighScoreService
{
    Task AddHighScoreAsync(HighScore highScore);
    Task<List<HighScore>> GetTopScoresAsync();
}

public class HighScoreService : IHighScoreService
{
    private readonly CloudTable _table;

    public HighScoreService(IConfiguration configuration)
    {
        string? connectionString = configuration["ConnectionStrings:AzureTableStorage"];
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        _table = tableClient.GetTableReference("HighScores");
        _table.CreateIfNotExistsAsync().Wait();
    }

    public async Task AddHighScoreAsync(HighScore highScore)
    {
        try
        {
            TableOperation insertOperation = TableOperation.Insert(highScore);
            _ = await _table.ExecuteAsync(insertOperation);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding high score: {ex.Message}");
        }
      
    }

    public async Task<List<HighScore>> GetTopScoresAsync()
    {
        TableQuery<HighScore> query = new TableQuery<HighScore>().Where(TableQuery.GenerateFilterConditionForInt("Score", QueryComparisons.GreaterThan, 0)).Take(10);
        TableQuerySegment<HighScore> segment = await _table.ExecuteQuerySegmentedAsync(query, null);
        return segment.Results;
    }
}
