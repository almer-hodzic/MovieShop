using Market.Domain.Entities.Identity;
using Market.Infrastructure.Database.Seeders;
using Market.Application.Modules.Auth.Commands.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Market.Tests;

[Collection("IntegrationTests")]
public sealed class PublicDemoSafetyTests
{
    private const string Password = "Fixture-only-public-demo!2026";

    [Fact]
    public async Task Shared_demo_login_stays_usable_and_cannot_change_credentials_or_delete_catalog()
    {
        await using var factory = new DemoFactory();
        using var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
                { ["PublicDemo:Password"] = Password }).Build();
            await PublicDemoSeeder.SeedAsync(db, config);
        }
        await Login(client, "portfolio-demo", Password);
        var email = await client.PutAsJsonAsync("Profile/email", new { email = "overwritten@example.invalid" });
        Assert.Equal(HttpStatusCode.Forbidden, email.StatusCode);
        var password = await client.PutAsJsonAsync("Profile/password", new { currentPassword = Password, newPassword = "Another-fixture-password!2026" });
        Assert.Equal(HttpStatusCode.Forbidden, password.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.DeleteAsync("Movies/1")).StatusCode);
        await Login(client, "portfolio-demo", Password);
        using var verify = factory.Services.CreateScope();
        var dbVerify = verify.ServiceProvider.GetRequiredService<DatabaseContext>();
        var user = await dbVerify.Users.SingleAsync(x => x.Username == "portfolio-demo");
        Assert.Equal("demo@movieshop.invalid", user.Email);
        Assert.False(user.IsAdmin || user.IsEmployee || user.IsManager);
    }

    [Fact]
    public async Task Ordinary_users_can_still_change_their_own_password_and_email()
    {
        await using var factory = new DemoFactory();
        using var client = factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N"); var username = "credential-test-" + suffix;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var user = new MarketUserEntity { Username = username, Email = suffix + "@example.invalid",
                Firstname = "Fixture", Lastname = "User", IsEnabled = true, IsEmailConfirmed = true,
                IsAdmin = false, IsManager = false, IsEmployee = false };
            user.PasswordHash = new PasswordHasher<MarketUserEntity>().HashPassword(user, Password);
            db.Users.Add(user); await db.SaveChangesAsync();
        }
        await Login(client, username, Password);
        (await client.PutAsJsonAsync("Profile/password", new { currentPassword = Password, newPassword = "Another-fixture-password!2026" })).EnsureSuccessStatusCode();
        (await client.PutAsJsonAsync("Profile/email", new { email = "updated-" + suffix + "@example.invalid" })).EnsureSuccessStatusCode();
        await Login(client, username, "Another-fixture-password!2026");
    }

    private static async Task Login(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("api/auth/login", new { email = username, password });
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoginCommandDto>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.AccessToken);
    }
    private sealed class DemoFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.UseEnvironment("IntegrationTests");
    }
}
