using ShoesShop.Application.DTOs.Payments;

namespace ShoesShop.Application.Interfaces.Services;

public interface IOrderService
{
    Task<OrderResponse> CheckoutAsync(long userId, CheckoutRequest request, string clientIpAddress);

    Task<OrderResponse> GetOrderDetailAsync(long userId, long orderId);

    Task<List<OrderResponse>> GetMyOrdersAsync(long userId);

    Task HandleVNPayCallbackAsync(Dictionary<string, string> queryParams);

    Task HandleMomoCallbackAsync(Dictionary<string, string> data);
}
