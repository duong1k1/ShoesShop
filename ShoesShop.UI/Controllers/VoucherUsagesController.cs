using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;
using ShoesShop.Application.DTOs;
namespace ShoesShop.UI.Controllers
{
    [ApiController]
    // Đường dẫn API lúc này sẽ là: api/VoucherUsages
    [Route("api/[controller]")]
    [Authorize] // Yêu cầu phải đăng nhập mới được dùng các API này
    public class VoucherUsagesController : ControllerBase
    {
        private readonly IVoucherUsageService _voucherService;

        // Hàm khởi tạo (Constructor) trùng với tên Class mới
        public VoucherUsagesController(IVoucherUsageService voucherService)
        {
            _voucherService = voucherService;
        }

        // DTO dùng để hứng dữ liệu truyền lên từ giao diện giỏ hàng
        //public record CheckVoucherDto(string VoucherCode, decimal TotalAmount);

        /// <summary>
        /// API kiểm tra và áp dụng thử mã giảm giá khi khách hàng nhập ở Giỏ hàng / Thanh toán
        /// </summary>
        [HttpPost("check-discount")]
        public async Task<IActionResult> CheckDiscount([FromBody] CheckVoucherDto model)
        {
            try
            {
                // Tự động lấy UserId của người dùng đang đăng nhập từ JWT Token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Unauthorized();

                long userId = long.Parse(userIdClaim.Value);

                // Gọi xuống Service để tính số tiền được giảm
                var discount = await _voucherService.CheckAndApplyVoucherAsync(userId, model.VoucherCode, model.TotalAmount);

                return Ok(new
                {
                    IsSuccess = true,
                    VoucherCode = model.VoucherCode,
                    DiscountAmount = discount,
                    Message = "Áp dụng mã giảm giá thành công!"
                });
            }
            catch (Exception ex)
            {
                // Nếu mã hết hạn, đã dùng rồi hoặc không đủ điều kiện, trả về lỗi kèm lý do
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// API lấy toàn bộ lịch sử sử dụng Voucher (Chỉ dành cho Admin)
        /// </summary>
        [HttpGet("history")]
        [Authorize(Roles = "Admin")] // Chỉ tài khoản có quyền Admin mới gọi được API này
        public async Task<IActionResult> GetHistory()
        {
            var histories = await _voucherService.GetAllHistoriesAsync();
            return Ok(histories);
        }
    }
}