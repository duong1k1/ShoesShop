namespace ShoesShop.Domain.Entities;

public class Review
{
    public long ReviewId { get; set; }

    public long ProductId { get; set; }

    public long UserId { get; set; }

    public long? OrderItemId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public string Status { get; set; } = "Visible";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Product Product { get; set; } = null!;

    public User User { get; set; } = null!;

    public OrderItem? OrderItem { get; set; }
}