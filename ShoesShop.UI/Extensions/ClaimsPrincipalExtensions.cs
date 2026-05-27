using System.Security.Claims;

namespace ShoesShop.UI.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Dùng để lấy UserId từ token của người dùng đã đăng nhập.
    /// Phương thức này sẽ tìm kiếm claim có loại là ClaimTypes.NameIdentifier và trả về giá trị của claim đó dưới dạng long. Nếu không tìm thấy claim hoặc giá trị của claim không hợp lệ, phương thức sẽ ném ra UnauthorizedAccessException với thông báo lỗi phù hợp.
    /// </summary>
    /// <param name="user">Đối tượng ClaimsPrincipal đại diện cho người dùng đã đăng nhập</param>
    /// <returns>UserId dưới dạng long</returns>
    /// <exception cref="UnauthorizedAccessException">Ném ra khi không tìm thấy UserId trong token hoặc giá trị không hợp lệ</exception>
    public static long GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("Không tìm thấy UserId trong token");

        return long.Parse(userId);
    }
}