using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Entities;

namespace ShoesShop.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        // 1. Hàm khởi tạo nhận cấu hình chuỗi kết nối từ Program.cs truyền xuống
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 2. Khai báo các Tập dữ liệu (Bảng) sẽ được ánh xạ xuống SQL Server
        public DbSet<User> Users { get; set; } = null!;

        // ĐÃ XÓA BỎ: DbSet<UserRole> tại đây vì database của bạn quản lý quyền trực tiếp trên bảng Users

        public DbSet<VoucherUsage> VoucherUsages { get; set; } = null!;

        // 3. Cấu hình ghi đè để Entity Framework hiểu cấu trúc bảng tùy biến
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // GIẢI PHÁP MẤU CHỐT: Ép EF Core bỏ qua hoàn toàn thực thể UserRole, xóa bỏ tận gốc lỗi sập 500
            modelBuilder.Ignore<UserRole>();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                // Khai báo chính xác khóa chính là cột UserId dưới database
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("UserId");

                // Ép kiểu rõ ràng cho cột Role và Phone để tránh EF tự suy diễn thành thực thể liên kết hoặc tạo cột ảo UserId1
                entity.Property(e => e.Role).HasColumnName("Role").HasColumnType("nvarchar(50)");
                entity.Property(e => e.PhoneNumber).HasColumnName("Phone");
            });
        }
    }
}