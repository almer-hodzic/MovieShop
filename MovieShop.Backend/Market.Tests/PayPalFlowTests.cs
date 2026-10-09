using Market.Application.Abstractions;
using Market.Application.Modules.Auth.Commands.Login;
using Market.Application.Modules.Payments.PayPal.Commands.CaptureOrder;
using Market.Application.Modules.Payments.PayPal.Commands.CreateOrder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Market.Tests;

[Collection("IntegrationTests")]
public sealed class PayPalFlowTests
{
    [Fact]
    public async Task PayPal_checkout_uses_current_users_cart_and_does_not_expose_access_token()
    {
        await using var factory = new PayPalFactory();
        using var anonymousClient = factory.CreateClient();
        var anonymousResponse = await anonymousClient.PostAsync("PayPal/CreateOrder", null);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        using var userClient = await CreateAuthenticatedClientAsync(factory, "user@market.local", "User123!");
        using var adminClient = await CreateAuthenticatedClientAsync(factory, "admin@market.local", "Admin123!");

        var emptyResponse = await userClient.DeleteAsync("ShoppingCart/clear");
        emptyResponse.EnsureSuccessStatusCode();

        emptyResponse = await userClient.PostAsync("PayPal/CreateOrder", null);
        Assert.Equal(HttpStatusCode.BadRequest, emptyResponse.StatusCode);

        var movie = await GetMovieAsync(factory);
        var addResponse = await userClient.PostAsJsonAsync("ShoppingCart/items", new
        {
            movieId = movie.Id,
            quantity = 2
        });
        addResponse.EnsureSuccessStatusCode();

        var createResponse = await userClient.PostAsync("PayPal/CreateOrder", null);
        createResponse.EnsureSuccessStatusCode();
        var createBody = await createResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain("accessToken", createBody, StringComparison.OrdinalIgnoreCase);

        var createOrder = JsonSerializer.Deserialize<CreatePayPalOrderCommandDto>(
            createBody,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(createOrder);
        Assert.False(string.IsNullOrWhiteSpace(createOrder.OrderId));
        Assert.Equal("USD", createOrder.CurrencyCode);
        Assert.Equal(movie.Price * 2, createOrder.Amount);

        var wrongUserCapture = await adminClient.PostAsJsonAsync("PayPal/CaptureOrder", new
        {
            orderId = createOrder.OrderId
        });
        Assert.Equal(HttpStatusCode.Forbidden, wrongUserCapture.StatusCode);

        var captureResponse = await userClient.PostAsJsonAsync("PayPal/CaptureOrder", new
        {
            orderId = createOrder.OrderId
        });
        captureResponse.EnsureSuccessStatusCode();
        var capture = await captureResponse.Content.ReadFromJsonAsync<CapturePayPalOrderCommandDto>();
        Assert.NotNull(capture);
        Assert.Equal("SUCCESS", capture.Status);

        var duplicateCapture = await userClient.PostAsJsonAsync("PayPal/CaptureOrder", new
        {
            orderId = createOrder.OrderId
        });
        Assert.Equal(HttpStatusCode.Conflict, duplicateCapture.StatusCode);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        WebApplicationFactory<Program> factory,
        string email,
        string password)
    {
        var client = factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password
        });
        loginResponse.EnsureSuccessStatusCode();

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(login);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AccessToken);

        return client;
    }

    private static async Task<(int Id, decimal Price)> GetMovieAsync(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var movie = await db.Movies
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.Price })
            .FirstAsync();

        return (movie.Id, movie.Price);
    }

    private sealed class PayPalFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTests");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IPayPalService>();
                services.AddSingleton<IPayPalService, FakePayPalService>();
            });
        }
    }

    private sealed class FakePayPalService : IPayPalService
    {
        public Task<PayPalCreateOrderResult> CreateOrderAsync(decimal amount, CancellationToken ct)
        {
            return Task.FromResult(new PayPalCreateOrderResult
            {
                OrderId = $"SANDBOX-TEST-{Guid.NewGuid():N}"
            });
        }

        public Task<PayPalCaptureOrderResult> CaptureOrderAsync(string orderId, CancellationToken ct)
        {
            return Task.FromResult(new PayPalCaptureOrderResult
            {
                IsCaptured = true,
                Status = "COMPLETED",
                Message = "Order captured successfully."
            });
        }

        public Task<string> GetOrderStatusAsync(string orderId, CancellationToken ct)
        {
            return Task.FromResult("CREATED");
        }
    }
}
