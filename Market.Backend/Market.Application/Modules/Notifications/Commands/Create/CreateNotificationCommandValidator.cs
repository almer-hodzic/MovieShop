namespace Market.Application.Modules.Notifications.Commands.Create;

public sealed class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.NotificationText)
            .NotEmpty().WithMessage("NotificationText is required.")
            .MaximumLength(NotificationEntity.Constraints.NotificationTextMaxLength)
            .WithMessage($"NotificationText can be at most {NotificationEntity.Constraints.NotificationTextMaxLength} characters long.");

        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("MovieId must be greater than 0.");
    }
}
