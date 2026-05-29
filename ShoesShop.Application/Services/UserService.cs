using Microsoft.EntityFrameworkCore;
using ShoesShop.Application.Interfaces.Common;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _context;

        public UserService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Trả về danh sách user chưa bị xóa mềm (Deleted At là null)
            return await _context.Users
                .Where(u => u.Status != "Deleted")
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> UpdateUserAsync(long userId, string fullName, string? phoneNumber, string? avatarUrl)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Status == "Deleted") return false;

            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            if (!string.IsNullOrEmpty(avatarUrl)) user.AvatarUrl = avatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeUserStatusAsync(long userId, string status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Status = status; // Active, Locked, Deleted
            if (status == "Deleted")
            {
                user.DeletedAt = DateTime.UtcNow; // Ghi nhận thời gian xóa mềm
            }
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}