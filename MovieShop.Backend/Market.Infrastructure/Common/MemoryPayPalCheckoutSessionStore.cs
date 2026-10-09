using Market.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Market.Infrastructure.Common;

public sealed class MemoryPayPalCheckoutSessionStore(IMemoryCache cache) : IPayPalCheckoutSessionStore
{
    private static readonly TimeSpan SessionTtl = TimeSpan.FromHours(3);

    public void Store(PayPalCheckoutSession session)
    {
        cache.Set(CacheKey(session.OrderId), session, SessionTtl);
    }

    public PayPalCheckoutSession? Get(string orderId)
    {
        return cache.Get<PayPalCheckoutSession>(CacheKey(orderId));
    }

    public void MarkCaptured(string orderId)
    {
        var session = Get(orderId);
        if (session is null)
            return;

        session.IsCaptured = true;
        Store(session);
    }

    private static string CacheKey(string orderId)
    {
        return $"paypal-checkout-session:{orderId}";
    }
}
