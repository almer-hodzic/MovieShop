using Market.Application.Modules.Sales.ShoppingCart.Commands.AddItem;
using Market.Application.Modules.Sales.ShoppingCart.Commands.Clear;
using Market.Application.Modules.Sales.ShoppingCart.Commands.MoveItemToSavedForLater;
using Market.Application.Modules.Sales.ShoppingCart.Commands.MoveSavedItemToCart;
using Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveItem;
using Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveSavedItem;
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

    [HttpPost("items/{itemId:int}/save-for-later")]
    public async Task MoveItemToSavedForLater(int itemId, CancellationToken ct)
    {
        await sender.Send(new MoveCartItemToSavedForLaterCommand { ItemId = itemId }, ct);
    }

    [HttpPost("saved-items/{itemId:int}/move-to-cart")]
    public async Task MoveSavedItemToCart(int itemId, CancellationToken ct)
    {
        await sender.Send(new MoveSavedItemToCartCommand { ItemId = itemId }, ct);
    }

    [HttpDelete("saved-items/{itemId:int}")]
    public async Task RemoveSavedItem(int itemId, CancellationToken ct)
    {
        await sender.Send(new RemoveSavedShoppingCartItemCommand { ItemId = itemId }, ct);
    }

    [HttpDelete("clear")]
    public async Task Clear(CancellationToken ct)
    {
        await sender.Send(new ClearShoppingCartCommand(), ct);
    }
}
