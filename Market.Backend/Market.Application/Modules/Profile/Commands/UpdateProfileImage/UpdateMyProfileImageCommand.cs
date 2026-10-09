namespace Market.Application.Modules.Profile.Commands.UpdateProfileImage;

public sealed class UpdateMyProfileImageCommand : IRequest
{
    public string ProfileImageBase64 { get; init; } = string.Empty;
}

