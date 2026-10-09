namespace Market.Application.Modules.Catalog.Directors.Queries.GetById;

public sealed class GetDirectorByIdQueryValidator : AbstractValidator<GetDirectorByIdQuery>
{
    public GetDirectorByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}