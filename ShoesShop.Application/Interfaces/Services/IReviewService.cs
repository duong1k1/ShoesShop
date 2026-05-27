using ShoesShop.Application.DTOs.Reviews;

namespace ShoesShop.Application.Interfaces.Services;

public interface IReviewService
{
    Task<ReviewResponse> CreateReviewAsync(long userId, CreateReviewRequest request);

    Task<ProductReviewSummaryResponse> GetProductReviewsAsync(long productId);

    Task<List<ReviewResponse>> GetMyReviewsAsync(long userId);

    Task<ReviewResponse> UpdateReviewAsync(long userId, long reviewId, UpdateReviewRequest request);

    Task DeleteReviewAsync(long userId, long reviewId);
}