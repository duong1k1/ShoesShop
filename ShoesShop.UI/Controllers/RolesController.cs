using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ShoesShop.Application.Interfaces.Services;

namespace ShoesShop.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        // Inject IRoleService vào Controller thông qua Constructor
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// API lấy danh sách tất cả các quyền hệ thống
        /// URL: GET /api/roles
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// API phân quyền hoặc thay đổi quyền cho một User cụ thể
        /// URL: PUT /api/roles/users/{userId}/assign-role
        /// </summary>
        [HttpPut("users/{userId}/assign-role")]
        public async Task<IActionResult> AssignRole(int userId, [FromQuery] string newRole)
        {
            // Kiểm tra tính hợp lệ của Role truyền vào đầu vào
            if (newRole != "Admin" && newRole != "Staff" && newRole != "Customer")
            {
                return BadRequest("Quyền không hợp lệ! Vui lòng chọn một trong các quyền: Admin, Staff, Customer.");
            }

            var isUpdated = await _roleService.UpdateUserRoleAsync(userId, newRole);

            if (!isUpdated)
            {
                return NotFound($"Không tìm thấy người dùng nào có ID = {userId} trong hệ thống.");
            }

            return Ok(new
            {
                Message = $"Đã phân quyền thành công cho User ID {userId} thành quyền: {newRole}."
            });
        }
    }
}