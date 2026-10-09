namespace Market.Application.Modules.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandDto
{
    public required int Id { get; init; }
    public required string Email { get; init; }
    public required bool IsEmailConfirmed { get; init; }
    public required DateTime EmailConfirmedAtUtc { get; init; }
}
