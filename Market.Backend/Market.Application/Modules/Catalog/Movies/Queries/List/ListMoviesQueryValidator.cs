namespace Market.Application.Modules.Catalog.Movies.Queries.List;

public sealed class ListMoviesQueryValidator : AbstractValidator<ListMoviesQuery>
{
    private static readonly string[] AllowedSortColumns = ["LastAddedMovies", "AverageScore"];

    public ListMoviesQueryValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(MovieEntity.Constraints.TitleMaxLength)
            .WithMessage($"Title filter can be at most {MovieEntity.Constraints.TitleMaxLength} characters long.")
            .When(x => x.Title is not null);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than 0.")
            .When(x => x.CategoryId is not null);

        RuleFor(x => x.PriceFrom)
            .GreaterThanOrEqualTo(0).WithMessage("PriceFrom must be non-negative.")
            .When(x => x.PriceFrom is not null);

        RuleFor(x => x.PriceTo)
            .GreaterThanOrEqualTo(0).WithMessage("PriceTo must be non-negative.")
            .When(x => x.PriceTo is not null);

        RuleFor(x => x)
            .Must(x => x.PriceFrom is null || x.PriceTo is null || x.PriceFrom <= x.PriceTo)
            .WithMessage("PriceFrom must be less than or equal to PriceTo.");

        RuleFor(x => x.SortedColumn)
            .Must(x => x is null || AllowedSortColumns.Contains(x))
            .WithMessage("SortedColumn is invalid.");
    }
}