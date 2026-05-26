using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.Domain.Entities
{
    [Table("Users")] // Ánh xạ chính xác vào bảng Users của bạn
    public class User
    {
        [Key]
        [Column("UserId")] // Ánh xạ biến Id vào đúng cột UserId dưới SQL
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string FullName { get; set; } = null!;

        [Column("Phone")] // Ánh xạ vào đúng cột Phone dưới SQL
        public string? PhoneNumber { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? AvatarUrl { get; set; } = "";

        public DateTime? UpdatedAt { get; set; } = null;

        public DateTime? DeletedAt { get; set; } = null;

        public int RoleId { get; set; } = 3;

        [Column("Role", TypeName = "nvarchar(50)")] // <--- THÊM DÒNG NÀY: Ép EF hiểu đây chỉ là cột chữ thường, không phải bảng liên kết!
        public string? Role { get; set; } = "Customer";
    }
}