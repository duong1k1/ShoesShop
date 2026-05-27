namespace ShoesShop.Domain.Entities;

public class PaymentTransaction
{
    public long PaymentTransactionId { get; set; }

    public long OrderId { get; set; }

    public string PaymentProvider { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? TransactionCode { get; set; }

    public string RequestId { get; set; } = string.Empty;

    public string? OrderInfo { get; set; }

    public string? PayUrl { get; set; }

    public string? ResponseCode { get; set; }

    public string? TransactionStatus { get; set; }

    public string Status { get; set; } = "Pending";

    public string? RawResponse { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Order Order { get; set; } = null!;
}