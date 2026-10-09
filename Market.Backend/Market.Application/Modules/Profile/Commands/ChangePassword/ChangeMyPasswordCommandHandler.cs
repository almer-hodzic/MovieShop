namespace Market.Application.Modules.Profile.Commands.ChangePassword;

public sealed class ChangeMyPasswordCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser,
    IPasswordHasher<MarketUserEntity> hasher)
    : IRequestHandler<ChangeMyPasswordCommand>
{
    public async Task Handle(ChangeMyPasswordCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, ct)
            ?? throw new MarketNotFoundException("Authenticated user profile not found.");

        var currentPasswordVerification = hasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (currentPasswordVerification == PasswordVerificationResult.Failed)
            throw new MarketConflictException("Current password is incorrect.");

        var newPasswordVerification = hasher.VerifyHashedPassword(user, user.PasswordHash, request.NewPassword);
        if (newPasswordVerification != PasswordVerificationResult.Failed)
            throw new MarketConflictException("New password must be different from current password.");

        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
        await ctx.SaveChangesAsync(ct);
    }
}

