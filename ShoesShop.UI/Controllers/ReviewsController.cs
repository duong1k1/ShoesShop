using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Application.DTOs.Reviews;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.UI.Extensions;

namespace ShoesShop.UI.Controllers;

[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // [Authorize]
    [HttpPost("api/reviews")]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        //var userId = User.GetUserId();

           var userId = 1;

        var result = await _reviewService.CreateReviewAsync(userId, request);

        return Ok(new
        {
            success = true,
            message = "Đánh giá sản phẩm thành công.",
            data = result
        });
    }

    [AllowAnonymous]
    [HttpGet("api/products/{productId:long}/reviews")]
    public async Task<IActionResult> GetProductReviews(long productId)
    {
        var result = await _reviewService.GetProductReviewsAsync(productId);

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đánh giá sản phẩm thành công.",
            data = result
        });
    }

    // [Authorize]
    [HttpGet("api/reviews/my")]
    public async Task<IActionResult> GetMyReviews()
    {
        //var userId = User.GetUserId();
        var userId = 1;
        var result = await _reviewService.GetMyReviewsAsync(userId);

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đánh giá của tôi thành công.",
            data = result
        });
    }

    // [Authorize]
    [HttpPut("api/reviews/{reviewId:long}")]
    public async Task<IActionResult> UpdateReview(
        long reviewId,
        [FromBody] UpdateReviewRequest request
    )
    {
        var userId = User.GetUserId();

        var result = await _reviewService.UpdateReviewAsync(userId, reviewId, request);

        return Ok(new
        {
            success = true,
            message = "Cập nhật đánh giá thành công.",
            data = result
        });
    }

    [Authorize]
    [HttpDelete("api/reviews/{reviewId:long}")]
    public async Task<IActionResult> DeleteReview(long reviewId)
    {
        var userId = User.GetUserId();

        await _reviewService.DeleteReviewAsync(userId, reviewId);

        return Ok(new
        {
            success = true,
            message = "Xóa đánh giá thành công."
        });
    }
}
