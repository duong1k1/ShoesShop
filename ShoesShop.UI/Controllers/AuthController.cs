using Microsoft.AspNetCore.Mvc;
using ShoesShop.Application.Interfaces.Services;
using System;
using System.Collections.Generic; // SỬA LỖI: Thêm thư viện này để không bị lỗi đỏ ở hàm List<string>
using System.Threading.Tasks;
using ShoesShop.Application.DTOs;

namespace ShoesShop.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Constructor thực hiện Inject AuthService
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// API Đăng ký tài khoản thành viên mới
        /// URL: POST /api/Auth/register
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var message = await _authService.RegisterAsync(model.Email, model.Password, model.FullName, model.PhoneNumber);

            if (message.Contains("tồn tại"))
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        /// <summary>
        /// API Đăng nhập hệ thống cấp Token JWT
        /// URL: POST /api/Auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var token = await _authService.LoginAsync(model.Email, model.Password);

            if (token == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác!" });
            }

            return Ok(new { Token = token, Message = "Đăng nhập thành công!" });
        }

        /// <summary>
        /// API Lấy danh sách quyền hệ thống phục vụ hiển thị lên giao diện
        /// URL: GET /api/Auth/roles
        /// </summary>
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            // ĐỒNG BỘ: Thêm "Staff" cho đủ bộ 3 quyền chuẩn tương ứng RoleId (1, 2, 3) dưới database của bạn
            var roles = new List<string> { "Admin", "Staff", "Customer" };
            return Ok(roles);
        }

        /// <summary>
        /// API Gán hoặc thay đổi quyền (Role) cho thành viên dựa theo ID
        /// URL: PUT /api/Auth/users/{id}/assign-role
        /// </summary>
        [HttpPut("users/{id}/assign-role")]
        public async Task<IActionResult> AssignRole(int id, [FromQuery] string newRole)
        {
            // SỬA LỖI ĐẦU VÀO: Chuyển từ [FromBody] sang [FromQuery] để khớp hoàn toàn với giao diện ô nhập trên Swagger của bạn.
            // Đồng thời bọc kiểm tra nhanh tính hợp lệ của chuỗi ký tự quyền gửi lên
            if (newRole != "Admin" && newRole != "Staff" && newRole != "Customer")
            {
                return BadRequest("Quyền không hợp lệ! Vui lòng chọn một trong ba quyền: Admin, Staff, Customer.");
            }

            // Gọi sang hàm xử lý database đã viết rất chuẩn trong AuthService
            var result = await _authService.UpdateUserRoleAsync(id, newRole);

            if (!result)
            {
                return NotFound(new { message = $"Không tìm thấy người dùng nào có ID = {id} trong hệ thống." });
            }

            return Ok(new { message = $"Đã phân quyền thành công cho User ID {id} thành quyền: {newRole}." });
        }
    }
}