namespace Market.Application.Modules.Catalog.Directors.Queries.List;

public sealed class ListDirectorsQueryValidator : AbstractValidator<ListDirectorsQuery>
{
    public ListDirectorsQueryValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name filter can be at most 200 characters long.")
            .When(x => x.Name is not null);
    }
}