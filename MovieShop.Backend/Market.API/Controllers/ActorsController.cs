using Market.Application.Modules.Catalog.Actors.Commands.Create;
using Market.Application.Modules.Catalog.Actors.Commands.Delete;
using Market.Application.Modules.Catalog.Actors.Commands.Update;
using Market.Application.Modules.Catalog.Actors.Queries.GetById;
using Market.Application.Modules.Catalog.Actors.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ActorsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<int>> Create(CreateActorCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task Update(int id, UpdateActorCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteActorCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetActorByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetActorByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    public async Task<PageResult<ListActorsQueryDto>> List([FromQuery] ListActorsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
