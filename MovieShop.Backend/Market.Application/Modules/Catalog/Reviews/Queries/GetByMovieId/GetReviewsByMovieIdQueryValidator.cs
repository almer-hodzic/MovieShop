namespace Market.Application.Modules.Catalog.Reviews.Queries.GetByMovieId;

public sealed class GetReviewsByMovieIdQueryValidator : AbstractValidator<GetReviewsByMovieIdQuery>
{
    public GetReviewsByMovieIdQueryValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("MovieId must be greater than 0.");

        RuleFor(x => x.UserName)
            .MaximumLength(200).WithMessage("UserName filter can be at most 200 characters long.")
            .When(x => x.UserName is not null);
    }
}
