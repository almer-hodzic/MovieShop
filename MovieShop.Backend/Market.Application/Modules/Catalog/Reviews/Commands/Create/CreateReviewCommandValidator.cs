namespace Market.Application.Modules.Catalog.Reviews.Commands.Create;

public sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("MovieId must be greater than 0.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MinimumLength(ReviewEntity.Constraints.CommentMinLength)
            .WithMessage($"Comment must be at least {ReviewEntity.Constraints.CommentMinLength} characters long.")
            .MaximumLength(ReviewEntity.Constraints.CommentMaxLength)
            .WithMessage($"Comment can be at most {ReviewEntity.Constraints.CommentMaxLength} characters long.");

        RuleFor(x => x.Score)
            .GreaterThan(0).WithMessage("Score must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Score must be less than or equal to 10.");
    }
}
