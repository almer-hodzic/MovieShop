using Market.Application.Abstractions;
using Market.Infrastructure.Database;
using Market.Domain.Entities.Sales;
namespace Market.Infrastructure.Common;

public sealed class DatabasePayPalCheckoutSessionStore(DatabaseContext db, TimeProvider clock)
    : IPayPalCheckoutSessionStore
{
    public async Task StoreAsync(PayPalCheckoutSession session, CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        // Bounded opportunistic cleanup also works when the host scales to zero.
        var expired = db.CheckoutSessions.Where(x => x.ExpiresAtUtc <= now)
            .OrderBy(x => x.ExpiresAtUtc).Take(100);
        if (db.Database.IsRelational())
            await expired.ExecuteDeleteAsync(ct); // Safe when multiple instances clean the same expired rows.
        else
            db.CheckoutSessions.RemoveRange(await expired.ToListAsync(ct));
        db.CheckoutSessions.Add(new CheckoutSessionEntity
        {
            OrderId = session.OrderId, UserId = session.UserId, Amount = session.Amount,
            CurrencyCode = session.CurrencyCode, IsCaptured = session.IsCaptured,
            CreatedAtUtc = now, ExpiresAtUtc = now.AddHours(3)
        });
        await db.SaveChangesAsync(ct);
    }

    public async Task<PayPalCheckoutSession?> GetAsync(string orderId, CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        return await db.CheckoutSessions.AsNoTracking()
            .Where(x => x.OrderId == orderId && x.ExpiresAtUtc > now)
            .Select(x => new PayPalCheckoutSession
            {
                OrderId = x.OrderId, UserId = x.UserId, Amount = x.Amount,
                CurrencyCode = x.CurrencyCode, IsCaptured = x.IsCaptured
            }).SingleOrDefaultAsync(ct);
    }

    public async Task MarkCapturedAsync(string orderId, CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var session = await db.CheckoutSessions.SingleOrDefaultAsync(
            x => x.OrderId == orderId && x.ExpiresAtUtc > now, ct);
        if (session is null) return;
        session.IsCaptured = true;
        await db.SaveChangesAsync(ct); // Do not extend the original expiry.
    }
}
