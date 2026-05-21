namespace ShoesShop.Domain.Entities;

public class ProductVariant
{
    public int ProductVariantId { get; set; }

    public int ProductId { get; set; }

    public string Size { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    public Product Product { get; set; } = null!;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}