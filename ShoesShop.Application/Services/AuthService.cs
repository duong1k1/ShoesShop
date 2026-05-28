using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly DbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(DbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<string> RegisterAsync(string email, string password, string fullName, string? phoneNumber)
        {
            // 1. Kiểm tra xem Email đã tồn tại chưa
            var isExist = await _context.Set<User>().AnyAsync(u => u.Email == email);
            if (isExist) return "Email đã tồn tại trên hệ thống!";

            // 2. Mã hóa mật khẩu bảo mật bằng BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // 3. Tạo đối tượng User mới bám sát cột thực tế trong database
            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                PhoneNumber = phoneNumber,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                RoleId = 3,          // Khớp hoàn toàn với RoleId = 3 (Khách hàng) dưới DB
                Role = "Customer"    // Khớp với chuỗi văn bản 'Customer'
            };

            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            // 4. TỰ ĐỘNG GỬI EMAIL THÔNG BÁO CHÀO MỪNG
            try
            {
                string emailSubject = "Chào mừng bạn đến với ShoesShop! 🎉";
                string emailBody = $@"
                    <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <h2 style='color: #2c3e50;'>Xin chào {fullName},</h2>
                        <p>Chúc mừng bạn đã đăng ký tài khoản thành công tại hệ thống <b>ShoesShop 2026</b>.</p>
                        <p>Thông tin tài khoản đăng nhập của bạn:</p>
                        <ul>
                            <li><b>Tài khoản (Email):</b> {email}</li>
                            <li><b>Trạng thái:</b> Đang hoạt động (Active)</li>
                        </ul>
                        <p>Bây giờ bạn đã có thể đăng nhập vào hệ thống và trải nghiệm mua sắm những mẫu giày mới nhất!</p>
                        <br/>
                        <p>Trân trọng,<br/><b>Đội ngũ ShoesShop.</b></p>
                    </div>";

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendEmailAsync(email, emailSubject, emailBody);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Email Error] Không gửi được mail: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Register Email Logic Error]: {ex.Message}");
            }

            return "Đăng ký thành công!";
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == email && u.Status == "Active");

            if (user == null) return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid) return null;

            var roles = new List<string> { user.Role ?? "Customer" };

            return GenerateJwtToken(user, roles);
        }

        private string GenerateJwtToken(User user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "Key_Bao_Mat_Mac_Dinh_ShoesShop_2026"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, string newRole)
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            int targetRoleId = 3;
            if (newRole == "Admin") targetRoleId = 1;
            else if (newRole == "Staff") targetRoleId = 2;

            user.Role = newRole;
            user.RoleId = targetRoleId;

            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }
    }
}