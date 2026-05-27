namespace ShoesShop.Application.DTOs.Carts;

public class UpdateCartItemRequest
{
    public long CartItemId { get; set; }

    public int Quantity { get; set; }
}