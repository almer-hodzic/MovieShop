using Market.Infrastructure.Database.Seeders;
using Microsoft.Extensions.Configuration;

namespace Market.Tests;

public sealed class RelationalDeploymentTests
{
    [Fact]
    public void All_seven_existing_migrations_are_discoverable()
    {
        using var db = new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer("Server=localhost;Database=MovieShopDeploymentTest;Integrated Security=true;TrustServerCertificate=true")
            .Options, TimeProvider.System);
        var migrations = db.Database.GetMigrations().ToList();
        Assert.Contains("20261010010000_AddSaveForLaterToCartItems", migrations);
        Assert.True(migrations.Count >= 7);
        Assert.False(db.Database.HasPendingModelChanges());
    }

    // Opt-in local SQL validation. The deployment runner supplies a fresh private test database.
    [SqlFact]
    public async Task Fresh_sql_migrations_and_public_seed_work_without_development_users()
    {
        var connection = Environment.GetEnvironmentVariable("MOVIESHOP_TEST_SQL");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connection);
        Assert.StartsWith("MovieShopDeploymentTest", builder.InitialCatalog);
        await using var db = new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connection).Options, TimeProvider.System);
        await db.Database.MigrateAsync();
        var applied = (await db.Database.GetAppliedMigrationsAsync()).ToList();
        Assert.True(applied.Count >= 7);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["PublicDemo:Password"] = "Fixture-only-relational-passphrase!2026"
        }).Build();
        await PublicDemoSeeder.SeedAsync(db, config);
        await PublicDemoSeeder.SeedAsync(db, config);
        db.ChangeTracker.Clear();
        var user = await db.Users.SingleAsync();
        Assert.False(user.IsAdmin || user.IsEmployee || user.IsManager);
        Assert.Equal(3, await db.Movies.CountAsync());
        Assert.True(await db.MovieActors.AnyAsync());
        Assert.True(await db.MovieCategories.AnyAsync());
        Assert.Empty(await db.Database.GetPendingMigrationsAsync());
        Assert.Equal(0, await db.CartItems.CountAsync()); // Exercises the formerly undiscoverable migration column.
    }
}

public sealed class SqlFactAttribute : FactAttribute
{
    public SqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MOVIESHOP_TEST_SQL")))
            Skip = "Set MOVIESHOP_TEST_SQL to a dedicated, fresh local SQL Server test database.";
    }
}
