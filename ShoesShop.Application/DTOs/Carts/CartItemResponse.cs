namespace ShoesShop.Application.DTOs.Carts;

public class CartItemResponse
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public int ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int StockQuantity { get; set; }

    public decimal TotalPrice { get; set; }
}