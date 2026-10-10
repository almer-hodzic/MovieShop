namespace Market.Application.Modules.Auth.Queries.CheckUsernameAvailability;

public sealed class CheckUsernameAvailabilityQuery : IRequest<CheckUsernameAvailabilityQueryDto>
{
    public string Username { get; init; } = string.Empty;
}
