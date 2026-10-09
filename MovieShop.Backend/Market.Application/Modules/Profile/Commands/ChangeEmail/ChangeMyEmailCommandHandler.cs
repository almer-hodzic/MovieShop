namespace Market.Application.Modules.Profile.Commands.ChangeEmail;

public sealed class ChangeMyEmailCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ChangeMyEmailCommand>
{
    public async Task Handle(ChangeMyEmailCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, ct)
            ?? throw new MarketNotFoundException("Authenticated user profile not found.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            throw new MarketConflictException("You already have this email.");

        var emailExists = await ctx.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id != userId && !x.IsDeleted && x.Email.ToLower() == normalizedEmail, ct);

        if (emailExists)
            throw new MarketConflictException("Email is already in use.");

        user.Email = normalizedEmail;
        await ctx.SaveChangesAsync(ct);
    }
}

