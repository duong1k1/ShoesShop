using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Application.DTOs.Carts;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.UI.Extensions;

namespace ShoesShop.UI.Controllers;

[ApiController]
[Route("api/cart")]
//[Authorize]

public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }


    /// <summary>
    /// Thêm sản phẩm vào giỏ hàng
    /// </summary>
    /// <param name="request">Thông tin sản phẩm cần thêm vào giỏ hàng</param>
    /// <returns>Kết quả thêm sản phẩm vào giỏ hàng</returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddCartItemRequest request)
    {
        //var userId = User.GetUserId();

        var userId = 1L; //test

        var result = await _cartService.AddToCartAsync(userId, request);

        return Ok(new
        {
            success = true,
            message = "Thêm sản phẩm vào giỏ hàng thành công",
            data = result
        });
    }

    /// <summary>
    /// Xem giỏ hàng hiện tại
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        // var userId = User.GetUserId();

        var userId = 1L; //test

        var result = await _cartService.GetCartAsync(userId);

        return Ok(new
        {
            success = true,
            message = "Lấy giỏ hàng thành công",
            data = result
        });
    }

    /// <summary>
    /// Cập nhật số lượng sản phẩm trong giỏ hàng
    /// </summary>
    /// <param name="request">Thông tin sản phẩm cần cập nhật số lượng</param>
    /// <returns>Kết quả cập nhật số lượng sản phẩm trong giỏ hàng</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemRequest request)
    {
        //var userId = User.GetUserId();

        var userId = 1L; //test

        var result = await _cartService.UpdateQuantityAsync(userId, request);

        return Ok(new
        {
            success = true,
            message = "Cập nhật số lượng thành công",
            data = result
        });
    }


    /// <summary>
    /// Xóa sản phẩm khỏi giỏ hàng  
    /// </summary>
    /// <param name="id">ID của cart item cần xóa</param>
    /// <returns>Kết quả xóa cart item</returns>
    [HttpDelete("item/{id:long}")]
    public async Task<IActionResult> DeleteCartItem(long id)
    {
        //var userId = User.GetUserId();

        var userId = 1L; //test

        await _cartService.DeleteCartItemAsync(userId, id);

        return Ok(new
        {
            success = true,
            message = "Xóa sản phẩm khỏi giỏ hàng thành công"
        });
    }

    /// <summary>
    /// Xóa toàn bộ giỏ hàng của người dùng, bao gồm tất cả các sản phẩm trong giỏ hàng. Sau khi thực hiện xóa, giỏ hàng sẽ trở về trạng thái rỗng. Phương thức này thường được sử dụng khi người dùng muốn hủy bỏ toàn bộ giỏ hàng hoặc sau khi hoàn tất đơn hàng để làm sạch giỏ hàng cho lần mua sắm tiếp theo.
    /// </summary>
    /// <returns>Kết quả xóa toàn bộ giỏ hàng</returns>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        //var userId = User.GetUserId();

        var userId = 1L; //test

        await _cartService.ClearCartAsync(userId);

        return Ok(new
        {
            success = true,
            message = "Xóa toàn bộ giỏ hàng thành công"
        });
    }
}
