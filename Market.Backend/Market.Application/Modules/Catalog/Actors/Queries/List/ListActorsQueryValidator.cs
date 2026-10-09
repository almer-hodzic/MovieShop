namespace Market.Application.Modules.Catalog.Actors.Queries.List;

public sealed class ListActorsQueryValidator : AbstractValidator<ListActorsQuery>
{
    public ListActorsQueryValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name filter can be at most 200 characters long.")
            .When(x => x.Name is not null);
    }
}