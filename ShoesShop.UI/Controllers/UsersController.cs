using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace ShoesShop.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public record UpdateUserDto(string FullName, string? PhoneNumber, string? AvatarUrl);
        public record ChangeStatusDto(string Status);

        // Chỉ những tài khoản đăng nhập với quyền Admin mới được lấy toàn bộ danh sách User
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize] // Bất kỳ ai đăng nhập đều xem được chi tiết (ví dụ: xem profile cá nhân)
        public async Task<IActionResult> GetById(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng này." });
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateUserDto model)
        {
            var success = await _userService.UpdateUserAsync(id, model.FullName, model.PhoneNumber, model.AvatarUrl);
            if (!success) return BadRequest(new { message = "Cập nhật thất bại." });
            return Ok(new { message = "Cập nhật hồ sơ thành công!" });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền khóa hoặc xóa tài khoản thành viên
        public async Task<IActionResult> ChangeStatus(long id, [FromBody] ChangeStatusDto model)
        {
            var success = await _userService.ChangeUserStatusAsync(id, model.Status);
            if (!success) return BadRequest(new { message = "Thay đổi trạng thái thất bại." });
            return Ok(new { message = $"Đã chuyển trạng thái tài khoản sang {model.Status}." });
        }
    }
}