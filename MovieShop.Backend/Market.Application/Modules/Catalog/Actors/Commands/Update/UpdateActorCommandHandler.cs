namespace Market.Application.Modules.Catalog.Actors.Commands.Update;

public sealed class UpdateActorCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateActorCommand>
{
    public async Task Handle(UpdateActorCommand request, CancellationToken ct)
    {
        var entity = await ctx.Actors
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Actor with Id {request.Id} not found.");

        entity.FirstName = request.FirstName.Trim();
        entity.LastName = request.LastName.Trim();
        entity.BirthDate = request.BirthDate.Date;
        entity.CountryId = request.CountryId;
        entity.ImdbLink = request.ImdbLink.Trim();
        entity.Biography = request.Biography.Trim();
        entity.Photo = DecodeBase64Image(request.PhotoBase64);

        await ctx.SaveChangesAsync(ct);
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