using System.ComponentModel.DataAnnotations;

namespace ShoesShop.Application.DTOs
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string FullName { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? PhoneNumber { get; set; }

        public string? AvatarUrl { get; set; }
    }
}