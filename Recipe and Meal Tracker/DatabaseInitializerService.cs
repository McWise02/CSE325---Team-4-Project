using Microsoft.EntityFrameworkCore;
using RecipeAndMealTracker.Data;

public class DatabaseInitializerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseInitializerService> _logger;

    public DatabaseInitializerService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseInitializerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Brief delay to ensure web server completes HTTP port binding
        await Task.Delay(1000, stoppingToken);

        _logger.LogInformation("Connecting to Azure SQL in background...");

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            await dbContext.Database.OpenConnectionAsync(stoppingToken);
            _logger.LogInformation("✅ Connection to Azure SQL succeeded!");
            await dbContext.Database.CloseConnectionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Azure SQL connection failed in background:");
        }
    }
}