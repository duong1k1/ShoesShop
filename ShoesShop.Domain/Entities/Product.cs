namespace ShoesShop.Domain.Entities;

public class Product
{
    public int ProductId { get; set; }

    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}