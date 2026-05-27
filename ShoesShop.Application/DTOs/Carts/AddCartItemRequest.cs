namespace ShoesShop.Application.DTOs.Carts;

public class AddCartItemRequest
{
    public long ProductId { get; set; }

    public string Size { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public int Quantity { get; set; }
}