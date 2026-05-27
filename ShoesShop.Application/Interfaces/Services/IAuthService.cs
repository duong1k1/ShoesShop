using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(string email, string password, string fullName, string? phoneNumber);
        Task<string?> LoginAsync(string email, string password);
        Task<bool> UpdateUserRoleAsync(int userId, string newRole);
    }
}