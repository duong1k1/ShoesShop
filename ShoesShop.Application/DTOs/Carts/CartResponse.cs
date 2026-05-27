namespace ShoesShop.Application.DTOs.Carts;

public class CartResponse
{
    public long CartId { get; set; }

    public long UserId { get; set; }

    public List<CartItemResponse> Items { get; set; } = new();

    public int TotalQuantity { get; set; }

    public decimal TotalAmount { get; set; }
}
