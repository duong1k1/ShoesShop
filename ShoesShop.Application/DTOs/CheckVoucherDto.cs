using System.ComponentModel.DataAnnotations;

namespace ShoesShop.Application.DTOs
{
    public class CheckVoucherDto
    {
        [Required(ErrorMessage = "Mã voucher không được để trống")]
        public string VoucherCode { get; set; } = null!;

        [Required(ErrorMessage = "Tổng tiền đơn hàng là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tổng tiền đơn hàng phải lớn hơn 0")]
        public decimal TotalAmount { get; set; }
    }
}