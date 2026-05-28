using Microsoft.Extensions.DependencyInjection;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.Application.Services;

namespace ShoesShop.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Đăng ký các API Service của bạn vào hệ thống
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            // Trả về services để có thể gọi chuỗi (Chaining) ở Program.cs
            return services;
        }
    }
}