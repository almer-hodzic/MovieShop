namespace Market.Application.Modules.Admin.Settings.Commands.UpdateMine;

public sealed class UpdateAdminSettingsCommandDto
{
    public required int Id { get; init; }
    public required string Email { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    public required bool IsAdmin { get; init; }
    public required bool IsEnabled { get; init; }
}
