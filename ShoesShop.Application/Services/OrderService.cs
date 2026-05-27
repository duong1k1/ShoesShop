using ShoesShop.Application.DTOs.Payments;
using ShoesShop.Application.Interfaces.Payments;
using ShoesShop.Application.Interfaces.Repositories;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.Domain.Constants;
using ShoesShop.Domain.Entities;
using ShoesShop.Domain.Exceptions;

namespace ShoesShop.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IVNPayService _vnPayService;
    private readonly IMomoService _momoService;

    public OrderService(
        IOrderRepository orderRepository,
        IVNPayService vnPayService,
        IMomoService momoService)
    {
        _orderRepository = orderRepository;
        _vnPayService = vnPayService;
        _momoService = momoService;
    }

    public async Task<OrderResponse> CheckoutAsync(
        long userId,
        CheckoutRequest request,
        string clientIpAddress)
    {
        return await _orderRepository.ExecuteInTransactionAsync(async () =>
        {
            var cart = await _orderRepository.GetActiveCartWithItemsAsync(userId);

            if (cart == null)
                throw new BadRequestException("Giỏ hàng không tồn tại hoặc đã được thanh toán.");

            if (!cart.CartItems.Any())
                throw new BadRequestException("Giỏ hàng đang trống.");

            foreach (var cartItem in cart.CartItems)
            {
                var variant = cartItem.ProductVariant;
                var product = variant.Product;

                if (product.Status != DomainConstants.ProductStatus.Active)
                    throw new BadRequestException($"Sản phẩm '{product.ProductName}' hiện không còn hoạt động.");

                if (variant.Status != DomainConstants.ProductVariantStatus.Active)
                    throw new BadRequestException($"Biến thể '{product.ProductName} - {variant.SizeValue} - {variant.ColorName}' hiện không còn hoạt động.");

                if (cartItem.Quantity > variant.StockQuantity)
                    throw new BadRequestException($"Sản phẩm '{product.ProductName}' không đủ tồn kho.");
            }

            var orderCode = await GenerateOrderCodeAsync();

            var orderItems = cart.CartItems.Select(cartItem =>
            {
                var variant = cartItem.ProductVariant;
                var product = variant.Product;

                var unitPrice = variant.PriceOverride
                    ?? product.SalePrice
                    ?? product.BasePrice;

                return new OrderItem
                {
                    ProductVariantId = variant.ProductVariantId,
                    ProductName = product.ProductName,
                    SKU = variant.SKU,
                    ColorName = variant.ColorName,
                    SizeValue = variant.SizeValue,
                    UnitPrice = unitPrice,
                    Quantity = cartItem.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
            }).ToList();

            var totalAmount = orderItems.Sum(x => x.UnitPrice * x.Quantity);
            var discountAmount = 0m;
            var shippingFee = request.ShippingFee;
            var finalAmount = totalAmount - discountAmount + shippingFee;

            var order = new Order
            {
                UserId = userId,
                OrderCode = orderCode,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                ShippingFee = shippingFee,
                FinalAmount = finalAmount,
                PaymentMethod = request.PaymentMethod.Trim(),
                PaymentStatus = DomainConstants.PaymentStatus.Pending,
                OrderStatus = DomainConstants.OrderStatus.Pending,
                RecipientName = request.RecipientName.Trim(),
                RecipientPhone = request.RecipientPhone.Trim(),
                ShippingAddress = request.ShippingAddress.Trim(),
                Note = request.Note?.Trim(),
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            foreach (var cartItem in cart.CartItems)
            {
                cartItem.ProductVariant.StockQuantity -= cartItem.Quantity;
                cartItem.ProductVariant.UpdatedAt = DateTime.UtcNow;
            }

            cart.Status = DomainConstants.CartStatus.CheckedOut;
            cart.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            var paymentRequest = new CreatePaymentRequest
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                Amount = order.FinalAmount,
                OrderInfo = $"Thanh toan don hang {order.OrderCode}",
                ClientIpAddress = clientIpAddress
            };

            string? paymentUrl = null;

            if (request.PaymentMethod == DomainConstants.PaymentMethod.COD)
            {
                await _orderRepository.AddPaymentTransactionAsync(new PaymentTransaction
                {
                    OrderId = order.OrderId,
                    PaymentProvider = DomainConstants.PaymentProvider.COD,
                    PaymentMethod = DomainConstants.PaymentMethod.COD,
                    Amount = order.FinalAmount,
                    RequestId = order.OrderCode,
                    OrderInfo = paymentRequest.OrderInfo,
                    Status = DomainConstants.PaymentTransactionStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else if (request.PaymentMethod == DomainConstants.PaymentMethod.VNPay)
            {
                var paymentResult = _vnPayService.CreatePaymentUrl(paymentRequest);

                paymentUrl = paymentResult.PaymentUrl;

                await _orderRepository.AddPaymentTransactionAsync(new PaymentTransaction
                {
                    OrderId = order.OrderId,
                    PaymentProvider = DomainConstants.PaymentProvider.VNPay,
                    PaymentMethod = DomainConstants.PaymentMethod.VNPay,
                    Amount = order.FinalAmount,
                    RequestId = paymentResult.RequestId,
                    OrderInfo = paymentRequest.OrderInfo,
                    PayUrl = paymentResult.PaymentUrl,
                    RawResponse = paymentResult.RawResponse,
                    Status = DomainConstants.PaymentTransactionStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else if (request.PaymentMethod == DomainConstants.PaymentMethod.Momo)
            {
                var paymentResult = await _momoService.CreatePaymentAsync(paymentRequest);

                paymentUrl = paymentResult.PaymentUrl;

                await _orderRepository.AddPaymentTransactionAsync(new PaymentTransaction
                {
                    OrderId = order.OrderId,
                    PaymentProvider = DomainConstants.PaymentProvider.Momo,
                    PaymentMethod = DomainConstants.PaymentMethod.Momo,
                    Amount = order.FinalAmount,
                    RequestId = paymentResult.RequestId,
                    OrderInfo = paymentRequest.OrderInfo,
                    PayUrl = paymentResult.PaymentUrl,
                    RawResponse = paymentResult.RawResponse,
                    Status = DomainConstants.PaymentTransactionStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                throw new BadRequestException("Phương thức thanh toán không hợp lệ.");
            }

            await _orderRepository.SaveChangesAsync();

            var createdOrder = await _orderRepository.GetOrderDetailAsync(order.OrderId, userId);

            if (createdOrder == null)
                throw new NotFoundException("Không tìm thấy đơn hàng vừa tạo.");

            var response = MapToOrderResponse(createdOrder);
            response.PaymentUrl = paymentUrl;

            return response;
        });
    }

    public async Task<OrderResponse> GetOrderDetailAsync(long userId, long orderId)
    {
        var order = await _orderRepository.GetOrderDetailAsync(orderId, userId);

        if (order == null)
            throw new NotFoundException("Không tìm thấy đơn hàng.");

        return MapToOrderResponse(order);
    }

    public async Task<List<OrderResponse>> GetMyOrdersAsync(long userId)
    {
        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

        return orders.Select(MapToOrderResponse).ToList();
    }

    public async Task HandleVNPayCallbackAsync(Dictionary<string, string> queryParams)
    {
        await _orderRepository.ExecuteInTransactionAsync(async () =>
        {
            var callback = _vnPayService.VerifyCallback(queryParams);

            if (!callback.IsValidSignature)
                throw new BadRequestException("Chữ ký VNPay không hợp lệ.");

            var transaction = await _orderRepository.GetPaymentTransactionByOrderCodeAsync(callback.OrderCode);

            if (transaction == null)
                throw new NotFoundException("Không tìm thấy giao dịch thanh toán.");

            if (transaction.Status == DomainConstants.PaymentTransactionStatus.Success)
                return true;

            transaction.TransactionCode = callback.TransactionCode;
            transaction.ResponseCode = callback.ResponseCode;
            transaction.TransactionStatus = callback.TransactionStatus;
            transaction.RawResponse = callback.RawData;
            transaction.UpdatedAt = DateTime.UtcNow;

            if (callback.IsSuccess)
            {
                transaction.Status = DomainConstants.PaymentTransactionStatus.Success;
                transaction.Order.PaymentStatus = DomainConstants.PaymentStatus.Paid;
                transaction.Order.OrderStatus = DomainConstants.OrderStatus.Confirmed;
            }
            else
            {
                transaction.Status = DomainConstants.PaymentTransactionStatus.Failed;
                transaction.Order.PaymentStatus = DomainConstants.PaymentStatus.Failed;
                transaction.Order.OrderStatus = DomainConstants.OrderStatus.Pending;
            }

            transaction.Order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.SaveChangesAsync();

            return true;
        });
    }

    public async Task HandleMomoCallbackAsync(Dictionary<string, string> data)
    {
        await _orderRepository.ExecuteInTransactionAsync(async () =>
        {
            var callback = _momoService.VerifyCallback(data);

            if (!callback.IsValidSignature)
                throw new BadRequestException("Chữ ký MoMo không hợp lệ.");

            var requestId = data.GetValueOrDefault("requestId") ?? string.Empty;

            var transaction = await _orderRepository.GetPaymentTransactionByRequestIdAsync(requestId);

            if (transaction == null)
                throw new NotFoundException("Không tìm thấy giao dịch thanh toán.");

            if (transaction.Status == DomainConstants.PaymentTransactionStatus.Success)
                return true;

            transaction.TransactionCode = callback.TransactionCode;
            transaction.ResponseCode = callback.ResponseCode;
            transaction.TransactionStatus = callback.TransactionStatus;
            transaction.RawResponse = callback.RawData;
            transaction.UpdatedAt = DateTime.UtcNow;

            if (callback.IsSuccess)
            {
                transaction.Status = DomainConstants.PaymentTransactionStatus.Success;
                transaction.Order.PaymentStatus = DomainConstants.PaymentStatus.Paid;
                transaction.Order.OrderStatus = DomainConstants.OrderStatus.Confirmed;
            }
            else
            {
                transaction.Status = DomainConstants.PaymentTransactionStatus.Failed;
                transaction.Order.PaymentStatus = DomainConstants.PaymentStatus.Failed;
                transaction.Order.OrderStatus = DomainConstants.OrderStatus.Pending;
            }

            transaction.Order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.SaveChangesAsync();

            return true;
        });
    }

    private async Task<string> GenerateOrderCodeAsync()
    {
        string orderCode;

        do
        {
            orderCode = $"OD{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";
        }
        while (await _orderRepository.IsOrderCodeExistsAsync(orderCode));

        return orderCode;
    }

    private static OrderResponse MapToOrderResponse(Order order)
    {
        return new OrderResponse
        {
            OrderId = order.OrderId,
            OrderCode = order.OrderCode,
            TotalAmount = order.TotalAmount,
            DiscountAmount = order.DiscountAmount,
            ShippingFee = order.ShippingFee,
            FinalAmount = order.FinalAmount,
            PaymentMethod = order.PaymentMethod,
            PaymentStatus = order.PaymentStatus,
            OrderStatus = order.OrderStatus,
            RecipientName = order.RecipientName,
            RecipientPhone = order.RecipientPhone,
            ShippingAddress = order.ShippingAddress,
            Note = order.Note,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(x => new OrderItemResponse
            {
                OrderItemId = x.OrderItemId,
                ProductVariantId = x.ProductVariantId,
                ProductName = x.ProductName,
                SKU = x.SKU,
                ColorName = x.ColorName,
                SizeValue = x.SizeValue,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                LineTotal = x.UnitPrice * x.Quantity
            }).ToList()
        };
    }
}