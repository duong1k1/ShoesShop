using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.Domain.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserId")]
        public long Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        [Column("Phone")]
        public string? PhoneNumber { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? AvatarUrl { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public int RoleId { get; set; } = 3;

        [Column("Role", TypeName = "nvarchar(50)")]
        public string? Role { get; set; } = "Customer";

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
