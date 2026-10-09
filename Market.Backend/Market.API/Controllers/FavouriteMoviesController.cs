using Market.Application.Modules.Catalog.FavouriteMovies.Commands.Add;
using Market.Application.Modules.Catalog.FavouriteMovies.Commands.Remove;
using Market.Application.Modules.Catalog.FavouriteMovies.Queries.GetMine;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class FavouriteMoviesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] AddFavouriteMovieCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return Ok(id);
    }

    [HttpDelete("{movieId:int}")]
    public async Task Remove(int movieId, CancellationToken ct)
    {
        await sender.Send(new RemoveFavouriteMovieCommand { MovieId = movieId }, ct);
    }

    [HttpGet("my")]
    public async Task<IReadOnlyList<GetMyFavouriteMoviesQueryDto>> GetMine(CancellationToken ct)
    {
        return await sender.Send(new GetMyFavouriteMoviesQuery(), ct);
    }
}
