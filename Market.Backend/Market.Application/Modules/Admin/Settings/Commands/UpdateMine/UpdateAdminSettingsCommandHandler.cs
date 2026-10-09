namespace Market.Application.Modules.Admin.Settings.Commands.UpdateMine;

public sealed class UpdateAdminSettingsCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateAdminSettingsCommand, UpdateAdminSettingsCommandDto>
{
    public async Task<UpdateAdminSettingsCommandDto> Handle(UpdateAdminSettingsCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated admin user is required.");

        var user = await ctx.Users
            .Where(x => x.Id == userId && !x.IsDeleted)
            .FirstOrDefaultAsync(ct)
            ?? throw new MarketNotFoundException("Authenticated user profile not found.");

        if (!user.IsAdmin)
            throw new MarketForbiddenException("Admin profile is required.");

        user.Firstname = request.Firstname.Trim();
        user.Lastname = request.Lastname.Trim();

        await ctx.SaveChangesAsync(ct);

        return new UpdateAdminSettingsCommandDto
        {
            Id = user.Id,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            IsAdmin = user.IsAdmin,
            IsEnabled = user.IsEnabled
        };
    }
}
