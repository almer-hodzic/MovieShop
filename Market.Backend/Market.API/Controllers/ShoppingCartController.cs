using Market.Application.Modules.Sales.ShoppingCart.Commands.AddItem;
using Market.Application.Modules.Sales.ShoppingCart.Commands.Clear;
using Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveItem;
using Market.Application.Modules.Sales.ShoppingCart.Commands.UpdateQuantity;
using Market.Application.Modules.Sales.ShoppingCart.Queries.GetMine;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class ShoppingCartController(ISender sender) : ControllerBase
{
    [HttpGet("my")]
    public async Task<GetMyShoppingCartQueryDto> GetMine(CancellationToken ct)
    {
        return await sender.Send(new GetMyShoppingCartQuery(), ct);
    }

    [HttpPost("items")]
    public async Task<ActionResult<int>> AddItem(AddItemToShoppingCartCommand command, CancellationToken ct)
    {
        int itemId = await sender.Send(command, ct);
        return Ok(itemId);
    }

    [HttpPut("items/{itemId:int}/quantity")]
    public async Task UpdateItemQuantity(int itemId, UpdateShoppingCartItemQuantityCommand command, CancellationToken ct)
    {
        command.ItemId = itemId;
        await sender.Send(command, ct);
    }

    [HttpDelete("items/{itemId:int}")]
    public async Task RemoveItem(int itemId, CancellationToken ct)
    {
        await sender.Send(new RemoveShoppingCartItemCommand { ItemId = itemId }, ct);
    }

    [HttpDelete("clear")]
    public async Task Clear(CancellationToken ct)
    {
        await sender.Send(new ClearShoppingCartCommand(), ct);
    }
}
