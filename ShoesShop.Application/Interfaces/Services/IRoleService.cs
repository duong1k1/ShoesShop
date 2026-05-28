using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Services
{
    public interface IRoleService
    {
        // 1. Lấy danh sách tất cả các quyền hệ thống (Dùng để hiển thị lên Dropdown/Select ở Front-end)
        Task<IEnumerable<string>> GetAllRolesAsync();

        // 2. Cập nhật hoặc phân quyền mới cho một User (Hàm mấu chốt để test API assign-role)
        Task<bool> UpdateUserRoleAsync(int userId, string newRole);

        // 3. Kiểm tra xem một User có đang nắm giữ quyền cụ thể nào đó không
        Task<bool> IsUserInRoleAsync(int userId, string roleName);
    }
}