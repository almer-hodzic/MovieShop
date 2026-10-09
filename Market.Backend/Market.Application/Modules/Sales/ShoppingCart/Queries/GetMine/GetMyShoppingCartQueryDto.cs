namespace Market.Application.Modules.Sales.ShoppingCart.Queries.GetMine;

public sealed class GetMyShoppingCartItemDto
{
    public required int ItemId { get; init; }
    public required int MovieId { get; init; }
    public required string MovieTitle { get; init; }
    public byte[]? MovieImage { get; init; }
    public required decimal UnitPrice { get; init; }
    public required int Quantity { get; init; }
    public required decimal TotalPrice { get; init; }
    public required DateTime AddedAt { get; init; }
}

public sealed class GetMyShoppingCartQueryDto
{
    public required int CartId { get; init; }
    public required int UserId { get; init; }
    public required DateTime LastModifiedAt { get; init; }
    public required IReadOnlyList<GetMyShoppingCartItemDto> Items { get; init; }
    public required int TotalQuantity { get; init; }
    public required decimal TotalAmount { get; init; }
}
