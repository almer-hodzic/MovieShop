namespace Market.Application.Modules.Admin.Settings.Commands.UpdateMine;

public sealed class UpdateAdminSettingsCommandValidator : AbstractValidator<UpdateAdminSettingsCommand>
{
    public UpdateAdminSettingsCommandValidator()
    {
        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("Firstname is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Firstname is required.")
            .MaximumLength(100).WithMessage("Firstname can be at most 100 characters long.");

        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Lastname is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Lastname is required.")
            .MaximumLength(100).WithMessage("Lastname can be at most 100 characters long.");
    }
}
