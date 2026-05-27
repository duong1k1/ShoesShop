using ShoesShop.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(long userId);
        Task<bool> UpdateUserAsync(long userId, string fullName, string? phoneNumber, string? avatarUrl);
        Task<bool> ChangeUserStatusAsync(long userId, string status);
    }
}