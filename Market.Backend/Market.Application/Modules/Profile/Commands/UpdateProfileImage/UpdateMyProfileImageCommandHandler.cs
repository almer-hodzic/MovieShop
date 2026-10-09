namespace Market.Application.Modules.Profile.Commands.UpdateProfileImage;

public sealed class UpdateMyProfileImageCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateMyProfileImageCommand>
{
    public async Task Handle(UpdateMyProfileImageCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, ct)
            ?? throw new MarketNotFoundException("Authenticated user profile not found.");

        user.ProfileImage = DecodeBase64Image(request.ProfileImageBase64);
        await ctx.SaveChangesAsync(ct);
    }

    private static byte[] DecodeBase64Image(string input)
    {
        try
        {
            var base64 = input;
            var commaIndex = input.IndexOf(',');
            if (commaIndex >= 0)
                base64 = input[(commaIndex + 1)..];

            return Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            throw new MarketConflictException("Invalid profile image format.");
        }
    }
}

