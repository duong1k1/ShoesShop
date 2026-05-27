namespace ShoesShop.Domain.Entities;

public class Product
{
    public long ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    public decimal BasePrice { get; set; }

    public decimal? SalePrice { get; set; }

    public string? Thumbnail { get; set; }

    public string Gender { get; set; } = "Unisex";

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}