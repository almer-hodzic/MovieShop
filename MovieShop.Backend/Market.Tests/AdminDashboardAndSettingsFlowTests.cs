using Market.Application.Modules.Admin.Dashboard.Queries.Get;
using Market.Application.Modules.Admin.Settings.Commands.UpdateMine;
using Market.Application.Modules.Admin.Settings.Queries.GetMine;
using Market.Application.Modules.Auth.Commands.Login;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Market.Tests;

[Collection("IntegrationTests")]
public sealed class AdminDashboardAndSettingsFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AdminDashboardAndSettingsFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("IntegrationTests"));
    }

    [Fact]
    public async Task Dashboard_and_settings_are_admin_only_and_use_real_persisted_data()
    {
        using var anonymousClient = _factory.CreateClient();
        var anonymousDashboard = await anonymousClient.GetAsync("Admin/Dashboard");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousDashboard.StatusCode);

        using var adminClient = await CreateAuthenticatedClientAsync("admin@market.local", "Admin123!");
        using var userClient = await CreateAuthenticatedClientAsync("user@market.local", "User123!");

        var userDashboard = await userClient.GetAsync("Admin/Dashboard");
        Assert.Equal(HttpStatusCode.Forbidden, userDashboard.StatusCode);

        var userSettings = await userClient.GetAsync("Admin/Settings");
        Assert.Equal(HttpStatusCode.Forbidden, userSettings.StatusCode);

        var expectedCounts = await GetExpectedDashboardCountsAsync();
        var dashboard = await adminClient.GetFromJsonAsync<GetAdminDashboardQueryDto>("Admin/Dashboard");
        Assert.NotNull(dashboard);
        Assert.Equal(expectedCounts.MoviesCount, dashboard.MoviesCount);
        Assert.Equal(expectedCounts.CategoriesCount, dashboard.CategoriesCount);
        Assert.Equal(expectedCounts.ActorsCount, dashboard.ActorsCount);
        Assert.Equal(expectedCounts.DirectorsCount, dashboard.DirectorsCount);
        Assert.Equal(expectedCounts.ReviewsCount, dashboard.ReviewsCount);
        Assert.Equal(expectedCounts.UsersCount, dashboard.UsersCount);
        Assert.Equal(expectedCounts.FavouritesCount, dashboard.FavouritesCount);
        Assert.Equal(expectedCounts.ActiveCartItemsCount, dashboard.ActiveCartItemsCount);
        Assert.Equal(expectedCounts.NotificationsCount, dashboard.NotificationsCount);

        var original = await adminClient.GetFromJsonAsync<GetAdminSettingsQueryDto>("Admin/Settings");
        Assert.NotNull(original);
        Assert.True(original.IsAdmin);

        var invalidResponse = await adminClient.PutAsJsonAsync("Admin/Settings", new
        {
            firstname = " ",
            lastname = original.Lastname
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var changedFirstname = original.Firstname.EndsWith(" RS1", StringComparison.Ordinal)
            ? original.Firstname.Replace(" RS1", string.Empty)
            : $"{original.Firstname} RS1";

        try
        {
            var updateResponse = await adminClient.PutAsJsonAsync("Admin/Settings", new
            {
                firstname = changedFirstname,
                lastname = original.Lastname
            });
            updateResponse.EnsureSuccessStatusCode();

            var updated = await updateResponse.Content.ReadFromJsonAsync<UpdateAdminSettingsCommandDto>();
            Assert.NotNull(updated);
            Assert.Equal(changedFirstname, updated.Firstname);
            Assert.Equal(original.Lastname, updated.Lastname);

            var reloaded = await adminClient.GetFromJsonAsync<GetAdminSettingsQueryDto>("Admin/Settings");
            Assert.NotNull(reloaded);
            Assert.Equal(changedFirstname, reloaded.Firstname);
            Assert.Equal(original.Lastname, reloaded.Lastname);
        }
        finally
        {
            await adminClient.PutAsJsonAsync("Admin/Settings", new
            {
                firstname = original.Firstname,
                lastname = original.Lastname
            });
        }
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

    private async Task<GetAdminDashboardQueryDto> GetExpectedDashboardCountsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        return new GetAdminDashboardQueryDto
        {
            MoviesCount = await db.Movies.AsNoTracking().CountAsync(x => !x.IsDeleted),
            CategoriesCount = await db.Categories.AsNoTracking().CountAsync(x => !x.IsDeleted),
            ActorsCount = await db.Actors.AsNoTracking().CountAsync(x => !x.IsDeleted),
            DirectorsCount = await db.Directors.AsNoTracking().CountAsync(x => !x.IsDeleted),
            ReviewsCount = await db.Reviews.AsNoTracking().CountAsync(x => !x.IsDeleted),
            UsersCount = await db.Users.AsNoTracking().CountAsync(x => !x.IsDeleted),
            FavouritesCount = await db.FavouriteMovies.AsNoTracking().CountAsync(x => !x.IsDeleted),
            ActiveCartItemsCount = await db.CartItems.AsNoTracking().CountAsync(x => !x.IsDeleted),
            NotificationsCount = await db.Notifications.AsNoTracking().CountAsync(x => !x.IsDeleted)
        };
    }
}
