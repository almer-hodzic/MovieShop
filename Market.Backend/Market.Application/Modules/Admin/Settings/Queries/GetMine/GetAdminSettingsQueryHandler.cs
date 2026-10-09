namespace Market.Application.Modules.Admin.Settings.Queries.GetMine;

public sealed class GetAdminSettingsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetAdminSettingsQuery, GetAdminSettingsQueryDto>
{
    public async Task<GetAdminSettingsQueryDto> Handle(GetAdminSettingsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated admin user is required.");

        return await ctx.Users
            .AsNoTracking()
            .Where(x => x.Id == userId && !x.IsDeleted && x.IsAdmin)
            .Select(x => new GetAdminSettingsQueryDto
            {
                Id = x.Id,
                Email = x.Email,
                Firstname = x.Firstname,
                Lastname = x.Lastname,
                IsAdmin = x.IsAdmin,
                IsEnabled = x.IsEnabled
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new MarketForbiddenException("Admin profile is required.");
    }
}
