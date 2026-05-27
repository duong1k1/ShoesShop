using Microsoft.EntityFrameworkCore;
using ShoesShop.Application.Interfaces.Repositories;
using ShoesShop.Domain.Entities;
using ShoesShop.Infrastructure.Persistence;

namespace ShoesShop.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetProductByIdAsync(long productId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.Status != "Deleted");
    }

    public async Task<OrderItem?> GetValidOrderItemForReviewAsync(
        long userId,
        long productId,
        long orderItemId
    )
    {
        return await _context.OrderItems
            .Include(x => x.Order)
            .Include(x => x.ProductVariant)
            .FirstOrDefaultAsync(x =>
                x.OrderItemId == orderItemId &&
                x.Order.UserId == userId &&
                x.Order.OrderStatus == "Delivered" &&
                x.ProductVariant.ProductId == productId);
    }

    public async Task<bool> HasReviewedOrderItemAsync(long orderItemId)
    {
        return await _context.Reviews
            .AnyAsync(x =>
                x.OrderItemId == orderItemId &&
                x.Status != "Deleted");
    }

    public async Task<Review?> GetReviewByIdAsync(long reviewId)
    {
        return await _context.Reviews
            .Include(x => x.Product)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.ReviewId == reviewId);
    }

    public async Task<Review?> GetReviewByIdAndUserIdAsync(long reviewId, long userId)
    {
        return await _context.Reviews
            .Include(x => x.Product)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.ReviewId == reviewId &&
                x.UserId == userId);
    }

    public async Task<List<Review>> GetReviewsByProductIdAsync(long productId)
    {
        return await _context.Reviews
            .Include(x => x.Product)
            .Include(x => x.User)
            .Where(x =>
                x.ProductId == productId &&
                x.Status == "Visible")
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Review>> GetReviewsByUserIdAsync(long userId)
    {
        return await _context.Reviews
            .Include(x => x.Product)
            .Include(x => x.User)
            .Where(x =>
                x.UserId == userId &&
                x.Status != "Deleted")
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddReviewAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}