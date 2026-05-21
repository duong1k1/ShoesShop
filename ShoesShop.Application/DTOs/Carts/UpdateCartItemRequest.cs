namespace ShoesShop.Application.DTOs.Carts;

public class UpdateCartItemRequest
{
    public int CartItemId { get; set; }

    public int Quantity { get; set; }
}