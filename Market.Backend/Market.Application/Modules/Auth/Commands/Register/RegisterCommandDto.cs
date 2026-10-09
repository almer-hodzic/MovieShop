namespace Market.Application.Modules.Auth.Commands.Register;

public sealed class RegisterCommandDto
{
    public required int Id { get; init; }
    public required string Email { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    public required bool IsEnabled { get; init; }
    public required bool IsEmployee { get; init; }
    public required bool IsEmailConfirmed { get; init; }
    public required bool EmailDeliveryFallbackUsed { get; init; }
    public string? EmailDeliveryMessage { get; init; }
}
