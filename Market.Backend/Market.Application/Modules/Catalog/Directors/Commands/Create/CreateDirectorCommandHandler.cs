namespace Market.Application.Modules.Catalog.Directors.Commands.Create;

public sealed class CreateDirectorCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateDirectorCommand, int>
{
    public async Task<int> Handle(CreateDirectorCommand request, CancellationToken ct)
    {
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var birthDate = request.BirthDate.Date;

        var entity = new DirectorEntity
        {
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
        };

        ctx.Directors.Add(entity);
        await ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}