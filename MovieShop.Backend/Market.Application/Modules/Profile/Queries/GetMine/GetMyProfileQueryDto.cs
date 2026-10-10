namespace Market.Application.Modules.Profile.Queries.GetMine;

public sealed class GetMyProfileQueryDto
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    public required bool IsAdmin { get; init; }
    public required bool IsManager { get; init; }
    public required bool IsEmployee { get; init; }
    public required bool IsEnabled { get; init; }
    public byte[]? ProfileImage { get; init; }
}

