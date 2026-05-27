namespace ShoesShop.Domain.Entities;

public class ProductVariant
{
    public long ProductVariantId { get; set; }

    public long ProductId { get; set; }

    public string ColorName { get; set; } = string.Empty;

    public string? ColorCode { get; set; }

    public string SizeValue { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string? Barcode { get; set; }

    public int StockQuantity { get; set; }

    public decimal? PriceOverride { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Product Product { get; set; } = null!;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}