using System;

namespace ShoesShop.Domain.Entities
{
    public class VoucherUsage
    {
        public long Id { get; set; }

        // Hãy chắc chắn chữ U và chữ I viết hoa
        public long UserId { get; set; }

        // Hãy chắc chắn chữ V và chữ C viết hoa
        public string VoucherCode { get; set; } = null!;

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}