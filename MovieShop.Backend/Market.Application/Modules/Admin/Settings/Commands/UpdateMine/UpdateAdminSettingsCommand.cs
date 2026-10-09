namespace Market.Application.Modules.Admin.Settings.Commands.UpdateMine;

public sealed class UpdateAdminSettingsCommand : IRequest<UpdateAdminSettingsCommandDto>
{
    public string Firstname { get; init; } = string.Empty;
    public string Lastname { get; init; } = string.Empty;
}
