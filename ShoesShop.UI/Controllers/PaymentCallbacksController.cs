using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Application.Interfaces.Services;

namespace ShoesShop.UI.Controllers;

[ApiController]
[Route("api/payment-callbacks")]
[AllowAnonymous]
public class PaymentCallbacksController : ControllerBase
{
    private readonly IOrderService _orderService;

    public PaymentCallbacksController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("vnpay-return")]
    public IActionResult VNPayReturn()
    {
        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

        var responseCode = queryParams.GetValueOrDefault("vnp_ResponseCode");
        var orderCode = queryParams.GetValueOrDefault("vnp_TxnRef");

        if (responseCode == "00")
        {
            return Ok(new
            {
                success = true,
                message = "Thanh toán VNPay thành công. Hệ thống sẽ xác nhận qua IPN.",
                orderCode
            });
        }

        return Ok(new
        {
            success = false,
            message = "Thanh toán VNPay không thành công.",
            orderCode,
            responseCode
        });
    }

    [HttpGet("vnpay-ipn")]
    public async Task<IActionResult> VNPayIpn()
    {
        try
        {
            var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

            await _orderService.HandleVNPayCallbackAsync(queryParams);

            return Ok(new
            {
                RspCode = "00",
                Message = "Confirm Success"
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                RspCode = "99",
                Message = ex.Message
            });
        }
    }

    [HttpGet("momo-return")]
    public IActionResult MomoReturn()
    {
        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

        var resultCode = queryParams.GetValueOrDefault("resultCode");
        var orderCode = queryParams.GetValueOrDefault("orderId");

        if (resultCode == "0")
        {
            return Ok(new
            {
                success = true,
                message = "Thanh toán MoMo thành công. Hệ thống sẽ xác nhận qua IPN.",
                orderCode
            });
        }

        return Ok(new
        {
            success = false,
            message = "Thanh toán MoMo không thành công.",
            orderCode,
            resultCode
        });
    }

    [HttpPost("momo-ipn")]
    public async Task<IActionResult> MomoIpn([FromBody] JsonElement body)
    {
        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(body.GetRawText())!
                .ToDictionary(x => x.Key, x => x.Value?.ToString() ?? string.Empty);

            await _orderService.HandleMomoCallbackAsync(data);

            return Ok(new
            {
                resultCode = 0,
                message = "Success"
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                resultCode = 99,
                message = ex.Message
            });
        }
    }
}