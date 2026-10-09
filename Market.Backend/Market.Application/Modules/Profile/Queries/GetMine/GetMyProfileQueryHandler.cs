namespace Market.Application.Modules.Profile.Queries.GetMine;

public sealed class GetMyProfileQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyProfileQuery, GetMyProfileQueryDto>
{
    public async Task<GetMyProfileQueryDto> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        return await ctx.Users
            .AsNoTracking()
            .Where(x => x.Id == userId && !x.IsDeleted)
            .Select(x => new GetMyProfileQueryDto
            {
                Id = x.Id,
                Email = x.Email,
                Firstname = x.Firstname,
                Lastname = x.Lastname,
                IsAdmin = x.IsAdmin,
                IsManager = x.IsManager,
                IsEmployee = x.IsEmployee,
                IsEnabled = x.IsEnabled,
                ProfileImage = x.ProfileImage
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new MarketNotFoundException("Authenticated user profile not found.");
    }
}

