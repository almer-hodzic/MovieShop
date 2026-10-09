using Market.Application.Modules.Catalog.Directors.Commands.Create;
using Market.Application.Modules.Catalog.Directors.Commands.Delete;
using Market.Application.Modules.Catalog.Directors.Commands.Update;
using Market.Application.Modules.Catalog.Directors.Queries.GetById;
using Market.Application.Modules.Catalog.Directors.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class DirectorsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<int>> Create(CreateDirectorCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task Update(int id, UpdateDirectorCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteDirectorCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetDirectorByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetDirectorByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    public async Task<PageResult<ListDirectorsQueryDto>> List([FromQuery] ListDirectorsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
