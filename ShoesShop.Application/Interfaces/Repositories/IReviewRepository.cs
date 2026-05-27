using ShoesShop.Domain.Entities;

namespace ShoesShop.Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<Product?> GetProductByIdAsync(long productId);

    Task<OrderItem?> GetValidOrderItemForReviewAsync(
        long userId,
        long productId,
        long orderItemId
    );

    Task<bool> HasReviewedOrderItemAsync(long orderItemId);

    Task<Review?> GetReviewByIdAsync(long reviewId);

    Task<Review?> GetReviewByIdAndUserIdAsync(long reviewId, long userId);

    Task<List<Review>> GetReviewsByProductIdAsync(long productId);

    Task<List<Review>> GetReviewsByUserIdAsync(long userId);

    Task AddReviewAsync(Review review);

    Task SaveChangesAsync();
}