using Market.API;
using Market.Infrastructure;
using Market.Shared.Options;
using Market.Infrastructure.Common;
using Market.Infrastructure.Database.Seeders;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Market.Tests;

public sealed class DeploymentSafetyTests
{
    [Fact]
    public async Task Public_demo_seed_is_idempotent_and_never_creates_staff_or_resets_passwords()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new DatabaseContext(options, TimeProvider.System);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["PublicDemo:Password"] = "Fixture-only-demo-passphrase!2026"
        }).Build();
        await PublicDemoSeeder.SeedAsync(db, config);
        var user = await db.Users.SingleAsync();
        var hash = user.PasswordHash;
        Assert.False(user.IsAdmin || user.IsEmployee || user.IsManager);
        Assert.True(user.IsEnabled && user.IsEmailConfirmed);
        Assert.Equal("portfolio-demo", user.Username);
        Assert.Equal(3, await db.Movies.CountAsync());
        Assert.True(await db.Categories.AnyAsync());
        await PublicDemoSeeder.SeedAsync(db, config);
        Assert.Equal(1, await db.Users.CountAsync());
        Assert.Equal(3, await db.Movies.CountAsync());
        Assert.Equal(hash, (await db.Users.SingleAsync()).PasswordHash);
        user.IsAdmin = true;
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => PublicDemoSeeder.SeedAsync(db, config));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("*")]
    [InlineData("https://*.vercel.app")]
    [InlineData("http://localhost:4200")]
    [InlineData("https://demo.example/path")]
    public void Production_cors_rejects_missing_wildcard_http_or_path_origins(string? origin)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Cors:AllowedOrigins:0"] = origin
        }).Build();
        Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddDeploymentProtection(config, new ProductionEnvironment()));
    }

    [Fact]
    public void Production_cors_allows_only_the_exact_frontend()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Cors:AllowedOrigins:0"] = "https://movieshop.example"
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDeploymentProtection(config, new ProductionEnvironment());
        using var provider = services.BuildServiceProvider();
        var policy = provider.GetRequiredService<IOptions<CorsOptions>>().Value.GetPolicy("Frontend")!;
        Assert.True(policy.IsOriginAllowed("https://movieshop.example"));
        Assert.False(policy.IsOriginAllowed("https://attacker.example"));
        Assert.False(policy.AllowAnyOrigin);
    }

    [Fact]
    public async Task Production_error_response_does_not_expose_exception_details()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.RequestServices = new ServiceCollection().AddLogging().AddOptions().BuildServiceProvider();
        var handler = new MarketExceptionHandler(NullLogger<MarketExceptionHandler>.Instance,
            new ProductionEnvironment());
        await handler.TryHandleAsync(context, new InvalidOperationException("PRIVATE-CONFIG-FIXTURE"), default);
        context.Response.Body.Position = 0;
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Equal(500, context.Response.StatusCode);
        Assert.DoesNotContain("PRIVATE-CONFIG-FIXTURE", response);
        Assert.DoesNotContain("InvalidOperationException", response);
    }

    [Theory]
    [InlineData("live", "https://api-m.paypal.com")]
    [InlineData("sandbox", "https://api-m.paypal.com")]
    [InlineData("sandbox", "https://untrusted.example")]
    public void PayPal_configuration_rejects_live_or_untrusted_endpoints(string mode, string baseUrl)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["PayPal:Mode"] = mode, ["PayPal:BaseUrl"] = baseUrl,
            ["PayPal:ClientId"] = "public-fixture", ["PayPal:ClientSecret"] = "private-fixture"
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(config, new ProductionEnvironment());
        using var provider = services.BuildServiceProvider();
        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<PayPalOptions>>().Value);
    }

    private sealed class ProductionEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "Market.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; }
            = new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
