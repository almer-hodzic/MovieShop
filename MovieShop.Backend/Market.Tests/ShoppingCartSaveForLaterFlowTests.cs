using Market.Application.Modules.Auth.Commands.Login;
using Market.Application.Modules.Sales.ShoppingCart.Queries.GetMine;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Market.Tests;

[Collection("IntegrationTests")]
public sealed class ShoppingCartSaveForLaterFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ShoppingCartSaveForLaterFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("IntegrationTests"));
    }

    [Fact]
    public async Task User_can_save_cart_item_for_later_and_move_it_back_without_affecting_active_total()
    {
        using var anonymousClient = _factory.CreateClient();
        var unauthorizedResponse = await anonymousClient.PostAsync("ShoppingCart/items/1/save-for-later", null);
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);

        await ClearUserCartAsync("user@market.local");
        using var userClient = await CreateAuthenticatedClientAsync("user@market.local", "User123!");
        using var adminClient = await CreateAuthenticatedClientAsync("admin@market.local", "Admin123!");

        var movie = await GetMovieAsync();
        var addResponse = await userClient.PostAsJsonAsync("ShoppingCart/items", new
        {
            movieId = movie.Id,
            quantity = 2
        });
        addResponse.EnsureSuccessStatusCode();

        var cart = await userClient.GetFromJsonAsync<GetMyShoppingCartQueryDto>("ShoppingCart/my");
        Assert.NotNull(cart);
        var activeItem = Assert.Single(cart.Items);
        Assert.Empty(cart.SavedForLaterItems);
        Assert.Equal(movie.Price * 2, cart.TotalAmount);

        var saveResponse = await userClient.PostAsync($"ShoppingCart/items/{activeItem.ItemId}/save-for-later", null);
        saveResponse.EnsureSuccessStatusCode();

        cart = await userClient.GetFromJsonAsync<GetMyShoppingCartQueryDto>("ShoppingCart/my");
        Assert.NotNull(cart);
        Assert.Empty(cart.Items);
        var savedItem = Assert.Single(cart.SavedForLaterItems);
        Assert.Equal(movie.Id, savedItem.MovieId);
        Assert.Equal(0, cart.TotalQuantity);
        Assert.Equal(0m, cart.TotalAmount);

        var wrongUserMoveResponse = await adminClient.PostAsync($"ShoppingCart/saved-items/{savedItem.ItemId}/move-to-cart", null);
        Assert.Equal(HttpStatusCode.NotFound, wrongUserMoveResponse.StatusCode);

        var moveBackResponse = await userClient.PostAsync($"ShoppingCart/saved-items/{savedItem.ItemId}/move-to-cart", null);
        moveBackResponse.EnsureSuccessStatusCode();

        cart = await userClient.GetFromJsonAsync<GetMyShoppingCartQueryDto>("ShoppingCart/my");
        Assert.NotNull(cart);
        activeItem = Assert.Single(cart.Items);
        Assert.Empty(cart.SavedForLaterItems);
        Assert.Equal(movie.Price * 2, cart.TotalAmount);

        saveResponse = await userClient.PostAsync($"ShoppingCart/items/{activeItem.ItemId}/save-for-later", null);
        saveResponse.EnsureSuccessStatusCode();

        cart = await userClient.GetFromJsonAsync<GetMyShoppingCartQueryDto>("ShoppingCart/my");
        Assert.NotNull(cart);
        savedItem = Assert.Single(cart.SavedForLaterItems);

        var removeSavedResponse = await userClient.DeleteAsync($"ShoppingCart/saved-items/{savedItem.ItemId}");
        removeSavedResponse.EnsureSuccessStatusCode();

        cart = await userClient.GetFromJsonAsync<GetMyShoppingCartQueryDto>("ShoppingCart/my");
        Assert.NotNull(cart);
        Assert.Empty(cart.Items);
        Assert.Empty(cart.SavedForLaterItems);
        Assert.Equal(0m, cart.TotalAmount);
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

    private async Task<(int Id, decimal Price)> GetMovieAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var movie = await db.Movies
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.Price })
            .FirstAsync();

        return (movie.Id, movie.Price);
    }

    private async Task ClearUserCartAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var userId = await db.Users
            .Where(x => x.Email == email)
            .Select(x => x.Id)
            .SingleAsync();

        var cart = await db.ShoppingCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (cart is null)
            return;

        db.CartItems.RemoveRange(cart.CartItems);
        await db.SaveChangesAsync();
    }
}
