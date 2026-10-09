using Market.Application.Abstractions;
using Market.Application.Common.Exceptions;
using Market.Shared.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Market.Infrastructure.Common;

public sealed class PayPalService(
    HttpClient httpClient,
    IOptions<PayPalOptions> options,
    ILogger<PayPalService> logger)
    : IPayPalService
{
    private readonly PayPalOptions _options = options.Value;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private string? _accessToken;
    private DateTime _tokenExpiresAtUtc;

    public async Task<PayPalCreateOrderResult> CreateOrderAsync(decimal amount, CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);
        var baseUrl = GetBaseUrl();

        var requestBody = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    amount = new
                    {
                        currency_code = _options.CurrencyCode,
                        value = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v2/checkout/orders");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        using var response = await httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("PayPal create order failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, body);
            throw new MarketConflictException("Error creating PayPal order.");
        }

        using var doc = JsonDocument.Parse(body);
        var orderId = doc.RootElement.GetProperty("id").GetString();
        if (string.IsNullOrWhiteSpace(orderId))
            throw new MarketConflictException("PayPal did not return an order id.");

        return new PayPalCreateOrderResult
        {
            OrderId = orderId,
        };
    }

    public async Task<PayPalCaptureOrderResult> CaptureOrderAsync(string orderId, CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);
        var baseUrl = GetBaseUrl();

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v2/checkout/orders/{orderId}/capture");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("PayPal capture order failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, body);

            return new PayPalCaptureOrderResult
            {
                IsCaptured = false,
                Status = "FAILED",
                Message = body.Contains("ORDER_ALREADY_CAPTURED", StringComparison.OrdinalIgnoreCase)
                    ? "PayPal order was already captured."
                    : body.Contains("ORDER_NOT_APPROVED", StringComparison.OrdinalIgnoreCase)
                        ? "PayPal order has not been approved yet."
                    : "PayPal order capture failed or was not approved."
            };
        }

        using var doc = JsonDocument.Parse(body);
        var status = doc.RootElement.GetProperty("status").GetString() ?? "UNKNOWN";
        var isCaptured = string.Equals(status, "COMPLETED", StringComparison.OrdinalIgnoreCase);

        return new PayPalCaptureOrderResult
        {
            IsCaptured = isCaptured,
            Status = status,
            Message = isCaptured
                ? "Order captured successfully."
                : $"PayPal order capture returned status '{status}'."
        };
    }

    public async Task<string> GetOrderStatusAsync(string orderId, CancellationToken ct)
    {
        var accessToken = await GetAccessTokenAsync(ct);
        var baseUrl = GetBaseUrl();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/v2/checkout/orders/{orderId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("PayPal order status failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, body);
            throw new MarketConflictException("Error retrieving PayPal order status.");
        }

        using var doc = JsonDocument.Parse(body);
        var status = doc.RootElement.GetProperty("status").GetString();

        if (string.IsNullOrWhiteSpace(status))
            throw new MarketConflictException("PayPal did not return order status.");

        return status;
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(_accessToken) && DateTime.UtcNow < _tokenExpiresAtUtc.AddSeconds(-30))
            return _accessToken!;

        await _tokenLock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrWhiteSpace(_accessToken) && DateTime.UtcNow < _tokenExpiresAtUtc.AddSeconds(-30))
                return _accessToken!;

            var baseUrl = GetBaseUrl();
            var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            using var response = await httpClient.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("PayPal token request failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, body);
                throw new MarketConflictException("Error requesting PayPal access token.");
            }

            using var doc = JsonDocument.Parse(body);
            var accessToken = doc.RootElement.GetProperty("access_token").GetString();
            var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();

            if (string.IsNullOrWhiteSpace(accessToken))
                throw new MarketConflictException("PayPal did not return an access token.");

            _accessToken = accessToken;
            _tokenExpiresAtUtc = DateTime.UtcNow.AddSeconds(expiresIn);

            return _accessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private string GetBaseUrl()
    {
        if (_options.Mode.Equals("live", StringComparison.OrdinalIgnoreCase)
            || _options.BaseUrl.Contains("api-m.paypal.com", StringComparison.OrdinalIgnoreCase)
            && !_options.BaseUrl.Contains("sandbox", StringComparison.OrdinalIgnoreCase))
        {
            throw new MarketConflictException("PayPal live endpoints are disabled for this sandbox integration.");
        }

        return string.IsNullOrWhiteSpace(_options.BaseUrl)
            ? "https://api-m.sandbox.paypal.com"
            : _options.BaseUrl.TrimEnd('/');
    }
}
