namespace Market.Application.Modules.Catalog.Actors.Queries.GetById;

public sealed class GetActorByIdQueryValidator : AbstractValidator<GetActorByIdQuery>
{
    public GetActorByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}