using Microsoft.EntityFrameworkCore;
using TruyenCV.Infrastructure.Persistence.Context;

namespace TruyenCV.API.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// Applies any pending Entity Framework Core migrations to the database on application startup.
    /// Also useful for seeding initial data after the schema is created.
    /// </summary>
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            logger.LogInformation("Checking for pending database migrations...");
            
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found.");
            }
        }
        catch (Exception ex)
        {
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var connection = dbContext.Database.GetDbConnection();
                var host = connection.DataSource;
                var database = connection.Database;

                logger.LogError(
                    ex,
                    "Database connection failed! Server: '{Host}', Database: '{Database}'. Error: {Message}",
                    host,
                    database,
                    ex.Message
                );
            }
            catch (Exception)
            {
                logger.LogError(ex, "Database connection failed! Error: {Message}", ex.Message);
            }

            logger.LogWarning("The application will continue to start, but database operations may fail.");
        }
    }
}
