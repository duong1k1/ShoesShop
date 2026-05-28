using ShoesShop.Application.DTOs.Reviews;
using ShoesShop.Application.Interfaces.Repositories;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.Domain.Entities;
using ShoesShop.Domain.Exceptions;

namespace ShoesShop.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewResponse> CreateReviewAsync(long userId, CreateReviewRequest request)
    {
        var product = await _reviewRepository.GetProductByIdAsync(request.ProductId);

        if (product == null)
            throw new NotFoundException("Sản phẩm không tồn tại.");

        if (product.Status != "Active")
            throw new BadRequestException("Sản phẩm hiện không được phép đánh giá.");

        var orderItem = await _reviewRepository.GetValidOrderItemForReviewAsync(
            userId,
            request.ProductId,
            request.OrderItemId
        );

        if (orderItem == null)
            throw new BadRequestException("Bạn chỉ có thể đánh giá sản phẩm đã mua và đơn hàng đã giao thành công.");

        var hasReviewed = await _reviewRepository.HasReviewedOrderItemAsync(request.OrderItemId);

        if (hasReviewed)
            throw new BadRequestException("Sản phẩm trong đơn hàng này đã được đánh giá.");

        var review = new Review
        {
            ProductId = request.ProductId,
            UserId = userId,
            OrderItemId = request.OrderItemId,
            Rating = request.Rating,
            Comment = string.IsNullOrWhiteSpace(request.Comment)
                ? null
                : request.Comment.Trim(),
            Status = "Visible",
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddReviewAsync(review);
        await _reviewRepository.SaveChangesAsync();

        var createdReview = await _reviewRepository.GetReviewByIdAsync(review.ReviewId);

        return MapToReviewResponse(createdReview!);
    }

    public async Task<ProductReviewSummaryResponse> GetProductReviewsAsync(long productId)
    {
        var product = await _reviewRepository.GetProductByIdAsync(productId);

        if (product == null)
            throw new NotFoundException("Sản phẩm không tồn tại.");

        var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);

        var responseReviews = reviews.Select(MapToReviewResponse).ToList();

        return new ProductReviewSummaryResponse
        {
            ProductId = productId,
            TotalReviews = responseReviews.Count,
            AverageRating = responseReviews.Count == 0
                ? 0
                : Math.Round(responseReviews.Average(x => x.Rating), 1),
            Reviews = responseReviews
        };
    }

    public async Task<List<ReviewResponse>> GetMyReviewsAsync(long userId)
    {
        var reviews = await _reviewRepository.GetReviewsByUserIdAsync(userId);

        return reviews.Select(MapToReviewResponse).ToList();
    }

    public async Task<ReviewResponse> UpdateReviewAsync(
        long userId,
        long reviewId,
        UpdateReviewRequest request
    )
    {
        var review = await _reviewRepository.GetReviewByIdAndUserIdAsync(reviewId, userId);

        if (review == null)
            throw new NotFoundException("Không tìm thấy đánh giá của bạn.");

        if (review.Status == "Deleted")
            throw new BadRequestException("Đánh giá đã bị xóa, không thể cập nhật.");

        review.Rating = request.Rating;
        review.Comment = string.IsNullOrWhiteSpace(request.Comment)
            ? null
            : request.Comment.Trim();
        review.UpdatedAt = DateTime.UtcNow;

        await _reviewRepository.SaveChangesAsync();

        return MapToReviewResponse(review);
    }

    public async Task DeleteReviewAsync(long userId, long reviewId)
    {
        var review = await _reviewRepository.GetReviewByIdAndUserIdAsync(reviewId, userId);

        if (review == null)
            throw new NotFoundException("Không tìm thấy đánh giá của bạn.");

        if (review.Status == "Deleted")
            return;

        review.Status = "Deleted";
        review.DeletedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;

        await _reviewRepository.SaveChangesAsync();
    }

    private static ReviewResponse MapToReviewResponse(Review review)
    {
        return new ReviewResponse
        {
            ReviewId = review.ReviewId,
            ProductId = review.ProductId,
            ProductName = review.Product.ProductName,
            UserId = review.UserId,
            UserName = review.User.FullName,
            OrderItemId = review.OrderItemId,
            Rating = review.Rating,
            Comment = review.Comment,
            Status = review.Status,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}