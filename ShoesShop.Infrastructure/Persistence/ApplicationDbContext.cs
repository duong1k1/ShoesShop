using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ShoesShop.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    /// <summary>
    /// Khai báo Db context cho ứng dụng, bao gồm các DbSet cho Cart, CartItem, Product và ProductVariant.
    /// Cấu hình các bảng
    /// Cấu hình primary key, các thuộc tính và quan hệ giữa các bảng trong phương thức OnModelCreating.
    /// Cấu hình relationship giữa Cart và CartItem là một-nhiều, giữa Product và ProductVariant là một-nhiều, và giữa CartItem và ProductVariant là nhiều-một.
    /// Cấu hình colunm type cho các trường giá tiền và cấu hình cascade delete cho quan hệ giữa Cart và CartItem để đảm bảo rằng khi một giỏ hàng bị xóa, tất cả các mục trong giỏ hàng cũng sẽ bị xóa theo.
    /// Cấu hình unique index trên CartItem để đảm bảo rằng một sản phẩm biến thể chỉ có thể xuất hiện một lần trong một giỏ hàng.
    /// </summary>
    /// <param name="options"></param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    // Thanh toán
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    //

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("Carts");

            entity.HasKey(x => x.CartId);

            entity.Property(x => x.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasMany(x => x.CartItems)
                .WithOne(x => x.Cart)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("CartItems");

            entity.HasKey(x => x.CartItemId);

            entity.Property(x => x.Quantity)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.CartId,
                x.ProductVariantId
            }).IsUnique();

            entity.HasOne(x => x.Cart)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ProductVariant)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(x => x.ProductId);

            entity.Property(x => x.ProductName)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Slug)
                .HasMaxLength(300)
                .IsRequired();

            entity.Property(x => x.BasePrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.SalePrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Status)
                .HasMaxLength(30)
                .IsRequired();
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.ToTable("ProductVariants");

            entity.HasKey(x => x.ProductVariantId);

            entity.Property(x => x.ColorName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.SizeValue)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.SKU)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.PriceOverride)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasOne(x => x.Product)
                .WithMany(x => x.ProductVariants)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // thanh toán
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");

            entity.HasKey(x => x.OrderId);

            entity.Property(x => x.OrderCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.ShippingFee)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.FinalAmount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.PaymentMethod)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.PaymentStatus)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.OrderStatus)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.RecipientName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.RecipientPhone)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.ShippingAddress)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.Note)
                .HasMaxLength(500);

            entity.HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");

            entity.HasKey(x => x.OrderItemId);

            entity.Property(x => x.ProductName)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.SKU)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.ColorName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.SizeValue)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.LineTotal)
                .HasComputedColumnSql("[UnitPrice] * [Quantity]", stored: true);

            entity.HasOne(x => x.ProductVariant)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.ToTable("PaymentTransactions");

            entity.HasKey(x => x.PaymentTransactionId);

            entity.Property(x => x.PaymentProvider)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.PaymentMethod)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.TransactionCode)
                .HasMaxLength(100);

            entity.Property(x => x.RequestId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.OrderInfo)
                .HasMaxLength(500);

            entity.Property(x => x.ResponseCode)
                .HasMaxLength(50);

            entity.Property(x => x.TransactionStatus)
                .HasMaxLength(50);

            entity.Property(x => x.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasOne(x => x.Order)
                .WithMany(x => x.PaymentTransactions)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Reviews");

            entity.HasKey(x => x.ReviewId);

            entity.Property(x => x.Rating)
                .IsRequired();

            entity.Property(x => x.Comment)
                .HasMaxLength(2000);

            entity.Property(x => x.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.HasOne(x => x.Product)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.OrderItem)
                .WithOne(x => x.Review)
                .HasForeignKey<Review>(x => x.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}