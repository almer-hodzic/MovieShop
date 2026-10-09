using Market.Application.Modules.Catalog.Reviews.Commands.Create;
using Market.Application.Modules.Catalog.Reviews.Commands.Delete;
using Market.Application.Modules.Catalog.Reviews.Queries.GetById;
using Market.Application.Modules.Catalog.Reviews.Queries.GetByMovieId;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ReviewsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateReviewCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteReviewCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetReviewByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetReviewByIdQuery { Id = id }, ct);
    }

    [HttpGet("by-movie")]
    public async Task<PageResult<GetReviewsByMovieIdQueryDto>> GetByMovieId([FromQuery] GetReviewsByMovieIdQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
