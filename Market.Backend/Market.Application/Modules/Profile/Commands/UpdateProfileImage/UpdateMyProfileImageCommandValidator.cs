namespace Market.Application.Modules.Profile.Commands.UpdateProfileImage;

public sealed class UpdateMyProfileImageCommandValidator : AbstractValidator<UpdateMyProfileImageCommand>
{
    public UpdateMyProfileImageCommandValidator()
    {
        RuleFor(x => x.ProfileImageBase64)
            .NotEmpty().WithMessage("Profile image is required.");
    }
}

