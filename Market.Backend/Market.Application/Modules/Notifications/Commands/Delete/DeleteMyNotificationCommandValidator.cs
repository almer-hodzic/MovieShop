namespace Market.Application.Modules.Notifications.Commands.Delete;

public sealed class DeleteMyNotificationCommandValidator : AbstractValidator<DeleteMyNotificationCommand>
{
    public DeleteMyNotificationCommandValidator()
    {
        RuleFor(x => x.NotificationId)
            .GreaterThan(0).WithMessage("NotificationId must be greater than 0.");
    }
}
