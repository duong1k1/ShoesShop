namespace ShoesShop.Domain.Entities;

public class CartItem
{
    public long CartItemId { get; set; }

    public long CartId { get; set; }

    public long ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Cart Cart { get; set; } = null!;

    public ProductVariant ProductVariant { get; set; } = null!;
}