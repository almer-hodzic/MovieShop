using Market.Infrastructure.Database.Seeders;
using Market.Infrastructure.Common;
using Market.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Market.Tests;

public sealed class RelationalDeploymentTests
{
    [Fact]
    public void Only_PostgreSQL_baseline_is_selected()
    {
        using var db = new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>()
            .UseNpgsql().Options, TimeProvider.System);
        var migrations = db.Database.GetMigrations().ToList();
        Assert.Single(migrations);
        Assert.EndsWith("PostgreSqlDeploymentBaseline", migrations[0]);
        Assert.False(db.Database.HasPendingModelChanges());
    }

    [PostgresFact]
    public async Task Checkout_checkpoint_survives_application_and_database_restart()
    {
        var cs = Environment.GetEnvironmentVariable("MOVIESHOP_TEST_POSTGRES")!;
        Assert.StartsWith("movieshop_deployment_test", new NpgsqlConnectionStringBuilder(cs).Database);
        await using var db = new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>()
            .UseNpgsql(cs).Options, TimeProvider.System);
        await db.Database.MigrateAsync();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["PublicDemo:Password"] = "Fixture-only-relational-passphrase!2026" }).Build();
        await PublicDemoSeeder.SeedAsync(db, config);
        var store = new DatabasePayPalCheckoutSessionStore(db, TimeProvider.System);
        var saved = await store.GetAsync("RESTART-CHECKPOINT", default);
        if (Environment.GetEnvironmentVariable("MOVIESHOP_EXPECT_RESTART") == "1")
        {
            Assert.NotNull(saved); Assert.True(saved.IsCaptured); Assert.Equal(9.87m, saved.Amount);
        }
        if (saved is null)
        {
            await store.StoreAsync(new PayPalCheckoutSession
            { OrderId = "RESTART-CHECKPOINT", UserId = (await db.Users.SingleAsync()).Id,
                Amount = 9.87m, CurrencyCode = "USD" }, default);
            await store.MarkCapturedAsync("RESTART-CHECKPOINT", default);
        }
    }

    [PostgresFact]
    public async Task Fresh_PostgreSQL_schema_seed_uniqueness_and_checkout_survive_new_contexts()
    {
        var connection = Environment.GetEnvironmentVariable("MOVIESHOP_TEST_POSTGRES")!;
        Assert.StartsWith("movieshop_deployment_test", new NpgsqlConnectionStringBuilder(connection).Database);
        var options = new DbContextOptionsBuilder<DatabaseContext>().UseNpgsql(connection).Options;
        int userId; string order = "PERSIST-" + Guid.NewGuid().ToString("N");
        await using (var db = new DatabaseContext(options, TimeProvider.System))
        {
            await db.Database.MigrateAsync();
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            { ["PublicDemo:Password"] = "Fixture-only-relational-passphrase!2026" }).Build();
            await PublicDemoSeeder.SeedAsync(db, config);
            await PublicDemoSeeder.SeedAsync(db, config);
            var user = await db.Users.SingleAsync(); userId = user.Id;
            Assert.False(user.IsAdmin || user.IsEmployee || user.IsManager);
            Assert.Equal(3, await db.Movies.CountAsync());
            Assert.True(await db.MovieActors.AnyAsync());
            Assert.Empty(await db.Database.GetPendingMigrationsAsync());
            Assert.Equal(1, await db.Users.CountAsync(x => x.Email == "DEMO@MOVIESHOP.INVALID"));
            Assert.Equal(1, await db.Movies.CountAsync(x => x.Title.ToLower().Contains("departed")));
            await new DatabasePayPalCheckoutSessionStore(db, TimeProvider.System).StoreAsync(new PayPalCheckoutSession
            { OrderId = order, UserId = userId, Amount = 12.34m, CurrencyCode = "USD" }, default);
        }
        await using (var db = new DatabaseContext(options, TimeProvider.System))
        {
            var store = new DatabasePayPalCheckoutSessionStore(db, TimeProvider.System);
            var session = await store.GetAsync(order, default);
            Assert.NotNull(session); Assert.Equal(userId, session.UserId); Assert.Equal(12.34m, session.Amount);
            Assert.False(session.IsCaptured);
            await store.MarkCapturedAsync(order, default);
        }
        await using (var db = new DatabaseContext(options, TimeProvider.System))
        {
            var store = new DatabasePayPalCheckoutSessionStore(db, TimeProvider.System);
            Assert.True((await store.GetAsync(order, default))!.IsCaptured);
            var row = await db.CheckoutSessions.SingleAsync(x => x.OrderId == order);
            row.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1); await db.SaveChangesAsync();
            Assert.Null(await store.GetAsync(order, default));
            await store.StoreAsync(new PayPalCheckoutSession
            { OrderId = order + "-NEXT", UserId = userId, Amount = 1m, CurrencyCode = "USD" }, default);
            Assert.False(await db.CheckoutSessions.AnyAsync(x => x.OrderId == order));
            // Uniqueness is case-insensitive at the DB boundary, not just in pre-insert queries.
            var original = await db.Users.SingleAsync();
            db.Users.Add(new Market.Domain.Entities.Identity.MarketUserEntity
            { Username = original.Username.ToUpperInvariant(), Email = "unique@example.invalid", PasswordHash = "fixture" });
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }
}

public sealed class PostgresFactAttribute : FactAttribute
{
    public PostgresFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MOVIESHOP_TEST_POSTGRES")))
            Skip = "Set MOVIESHOP_TEST_POSTGRES to a dedicated fresh PostgreSQL test database.";
    }
}
