using System;

namespace ShoesShop.Domain.Entities
{
    public class UserRole
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Các thuộc tính liên kết quan hệ (Navigation Properties) nếu có
     //   public virtual User User { get; set; } = null!;
    }
}