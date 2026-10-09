using Market.Application.Modules.Auth.Commands.Login;
using Market.Application.Modules.Notifications.Queries.GetMine;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Tests;

[Collection("IntegrationTests")]
public sealed class NotificationsFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public NotificationsFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("IntegrationTests"));
    }

    [Fact]
    public async Task Notifications_are_isolated_markable_and_deletable_for_current_user()
    {
        using var anonymousClient = _factory.CreateClient();
        var anonymousResponse = await anonymousClient.GetAsync("Notifications/my");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        using var adminClient = await CreateAuthenticatedClientAsync("admin@market.local", "Admin123!");
        using var userClient = await CreateAuthenticatedClientAsync("user@market.local", "User123!");

        var movieId = await GetMovieIdAsync();
        var notificationText = $"Integration notification {Guid.NewGuid():N}";

        var createResponse = await adminClient.PostAsJsonAsync("Notifications", new
        {
            notificationText,
            movieId
        });
        createResponse.EnsureSuccessStatusCode();

        var notificationId = await createResponse.Content.ReadFromJsonAsync<int>();

        var adminMine = await adminClient.GetFromJsonAsync<IReadOnlyList<GetMyNotificationsQueryDto>>("Notifications/my");
        Assert.NotNull(adminMine);
        Assert.DoesNotContain(adminMine, x => x.NotificationId == notificationId);

        var userMine = await userClient.GetFromJsonAsync<IReadOnlyList<GetMyNotificationsQueryDto>>("Notifications/my");
        Assert.NotNull(userMine);
        var notification = Assert.Single(userMine.Where(x => x.NotificationId == notificationId));
        Assert.Equal(notificationText, notification.NotificationText);
        Assert.False(notification.IsRead);
        Assert.Equal(movieId, notification.MovieId);

        var userCreateResponse = await userClient.PostAsJsonAsync("Notifications", new
        {
            notificationText = "Normal user should not create this",
            movieId
        });
        Assert.Equal(HttpStatusCode.Forbidden, userCreateResponse.StatusCode);

        var adminMarkReadResponse = await adminClient.PutAsJsonAsync($"Notifications/{notificationId}/read", new { });
        Assert.Equal(HttpStatusCode.NotFound, adminMarkReadResponse.StatusCode);

        var markReadResponse = await userClient.PutAsJsonAsync($"Notifications/{notificationId}/read", new { });
        Assert.Equal(HttpStatusCode.NoContent, markReadResponse.StatusCode);

        userMine = await userClient.GetFromJsonAsync<IReadOnlyList<GetMyNotificationsQueryDto>>("Notifications/my");
        Assert.NotNull(userMine);
        notification = Assert.Single(userMine.Where(x => x.NotificationId == notificationId));
        Assert.True(notification.IsRead);

        var deleteResponse = await userClient.DeleteAsync($"Notifications/{notificationId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        userMine = await userClient.GetFromJsonAsync<IReadOnlyList<GetMyNotificationsQueryDto>>("Notifications/my");
        Assert.NotNull(userMine);
        Assert.DoesNotContain(userMine, x => x.NotificationId == notificationId);

        var deleteAgainResponse = await userClient.DeleteAsync($"Notifications/{notificationId}");
        Assert.Equal(HttpStatusCode.NotFound, deleteAgainResponse.StatusCode);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
    {
        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password
        });
        loginResponse.EnsureSuccessStatusCode();

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(login);
        Assert.False(login.RequiresTwoFactor);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AccessToken);

        return client;
    }

    private async Task<int> GetMovieIdAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        return await db.Movies
            .OrderBy(x => x.Id)
            .Select(x => x.Id)
            .FirstAsync();
    }
}
