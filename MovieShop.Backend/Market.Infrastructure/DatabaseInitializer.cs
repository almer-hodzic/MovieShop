using Market.Infrastructure.Database;
using Market.Infrastructure.Database.Seeders;
using Market.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Market.Infrastructure;

public static class DatabaseInitializer
{
    /// <summary>
    /// Centralized migration and seeding.
    /// </summary>
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        IHostEnvironment env,
        IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DatabaseInitializer));

        if (env.IsTest())
        {
            await ctx.Database.EnsureCreatedAsync();
            await DynamicDataSeeder.SeedAsync(ctx);
            return;
        }

        var applyMigrationsOnStartup = configuration
            .GetValue<bool?>("DatabaseStartup:ApplyMigrationsOnStartup")
            ?? !env.IsDevelopment();

        var seedOnStartup = configuration
            .GetValue<bool?>("DatabaseStartup:SeedOnStartup")
            ?? false;

        if (applyMigrationsOnStartup)
        {
            logger.LogInformation("Applying database migrations on startup.");
            await ctx.Database.MigrateAsync();
        }
        else
        {
            logger.LogInformation("Skipping database migrations on startup.");
        }

        if (env.IsDevelopment() && seedOnStartup)
        {
            logger.LogInformation("Applying development database seed on startup.");
            await DynamicDataSeeder.SeedAsync(ctx);
        }
        else if (env.IsDevelopment())
        {
            logger.LogInformation("Skipping development database seed on startup.");
        }
    }
}
