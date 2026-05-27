using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Application.DTOs.Payments;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.UI.Extensions;

namespace ShoesShop.UI.Controllers;

[ApiController]
[Route("api/orders")]
//[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        //var userId = User.GetUserId();

        var userId = 1L;

        var clientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        var result = await _orderService.CheckoutAsync(userId, request, clientIpAddress);

        return Ok(new
        {
            success = true,
            message = "Đặt hàng thành công.",
            data = result
        });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        //var userId = User.GetUserId();
        var userId = 1L;

        var result = await _orderService.GetMyOrdersAsync(userId);

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách đơn hàng thành công.",
            data = result
        });
    }

    [HttpGet("{orderId:long}")]
    public async Task<IActionResult> GetOrderDetail(long orderId)
    {
        //var userId = User.GetUserId();
        var userId = 1L;

        var result = await _orderService.GetOrderDetailAsync(userId, orderId);

        return Ok(new
        {
            success = true,
            message = "Lấy chi tiết đơn hàng thành công.",
            data = result
        });
    }
}