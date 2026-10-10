namespace Market.Application.Modules.Auth.Queries.CheckUsernameAvailability;

public sealed class CheckUsernameAvailabilityQueryHandler(IAppDbContext ctx)
    : IRequestHandler<CheckUsernameAvailabilityQuery, CheckUsernameAvailabilityQueryDto>
{
    public async Task<CheckUsernameAvailabilityQueryDto> Handle(CheckUsernameAvailabilityQuery request, CancellationToken ct)
    {
        var normalizedUsername = request.Username.Trim().ToLowerInvariant();

        var exists = await ctx.Users
            .AsNoTracking()
            .AnyAsync(x => !x.IsDeleted && x.Username.ToLower() == normalizedUsername, ct);

        return new CheckUsernameAvailabilityQueryDto
        {
            Available = !exists,
        };
    }
}
