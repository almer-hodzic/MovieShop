namespace Market.Application.Modules.Catalog.Actors.Commands.Create;

public sealed class CreateActorCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateActorCommand, int>
{
    public async Task<int> Handle(CreateActorCommand request, CancellationToken ct)
    {
        var entity = new ActorEntity
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            BirthDate = request.BirthDate.Date,
            CountryId = request.CountryId,
            ImdbLink = request.ImdbLink.Trim(),
            Biography = request.Biography.Trim(),
            Photo = DecodeBase64Image(request.PhotoBase64),
        };

        ctx.Actors.Add(entity);
        await ctx.SaveChangesAsync(ct);

        return entity.Id;
    }

    private static byte[] DecodeBase64Image(string input)
    {
        try
        {
            var base64 = input;
            var commaIndex = input.IndexOf(',');
            if (commaIndex >= 0)
                base64 = input[(commaIndex + 1)..];

            return Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            throw new MarketConflictException("Invalid photo format.");
        }
    }
}