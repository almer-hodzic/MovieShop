using Market.Application.Modules.Catalog.Movies.Commands.Create;
using Market.Application.Modules.Catalog.Movies.Commands.Delete;
using Market.Application.Modules.Catalog.Movies.Commands.Update;
using Market.Application.Modules.Catalog.Movies.Queries.GetById;
using Market.Application.Modules.Catalog.Movies.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class MoviesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<int>> Create(CreateMovieCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task Update(int id, UpdateMovieCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteMovieCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetMovieByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetMovieByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    public async Task<PageResult<ListMoviesQueryDto>> List([FromQuery] ListMoviesQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
