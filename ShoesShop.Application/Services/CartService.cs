using ShoesShop.Application.DTOs.Carts;
using ShoesShop.Application.Interfaces.Repositories;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.Domain.Entities;
using ShoesShop.Domain.Exceptions;

namespace ShoesShop.Application.Services;

public class CartService : ICartService
{
    private const string ActiveStatus = "Active";

    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    /// <summary>
    /// Thêm sản phẩm vào giỏ hàng của người dùng. Nếu sản phẩm đã tồn tại trong giỏ, cập nhật số lượng.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="BadRequestException"></exception>
    public async Task<CartResponse> AddToCartAsync(long userId, AddCartItemRequest request)
    {
        var variant = await _cartRepository.GetVariantAsync(
            request.ProductId,
            request.Size.Trim(),
            request.Color.Trim()
        );

        if (variant == null)
            throw new NotFoundException("Biến thể sản phẩm không tồn tại.");

        if (variant.Status != ActiveStatus)
            throw new BadRequestException("Biến thể sản phẩm đã ngừng bán.");

        if (variant.Product.Status != ActiveStatus)
            throw new BadRequestException("Sản phẩm đã ngừng bán.");

        if (request.Quantity > variant.StockQuantity)
            throw new BadRequestException("Số lượng thêm vào vượt quá số lượng tồn kho.");

        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            cart = await _cartRepository.CreateCartAsync(userId);
        }

        var cartItem = await _cartRepository.GetCartItemAsync(
            cart.CartId,
            variant.ProductVariantId
        );

        if (cartItem != null)
        {
            var newQuantity = cartItem.Quantity + request.Quantity;

            if (newQuantity > variant.StockQuantity)
                throw new BadRequestException("Tổng số lượng trong giỏ vượt quá số lượng tồn kho.");

            cartItem.Quantity = newQuantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            cartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductVariantId = variant.ProductVariantId,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddCartItemAsync(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        return await GetCartAsync(userId);
    }

    /// <summary>
    /// Lấy chi tiết giỏ hàng của người dùng, bao gồm danh sách sản phẩm, số lượng, giá tiền và tổng tiền.
    /// Nếu giỏ hàng trống, trả về thông tin giỏ hàng với danh sách sản phẩm rỗng và tổng tiền bằng 0.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<CartResponse> GetCartAsync(long userId)
    {
        var cart = await _cartRepository.GetCartDetailByUserIdAsync(userId);

        if (cart == null)
        {
            return new CartResponse
            {
                CartId = 0,
                UserId = userId,
                Items = new List<CartItemResponse>(),
                TotalQuantity = 0,
                TotalAmount = 0
            };
        }

        return MapToCartResponse(cart);
    }

    /// <summary>
    /// Cập nhật số lượng của một sản phẩm trong giỏ hàng.
    /// Kiểm tra tồn tại của sản phẩm, biến thể, và số lượng tồn kho trước khi cập nhật.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="BadRequestException"></exception>
    public async Task<CartResponse> UpdateQuantityAsync(
        long userId,
        UpdateCartItemRequest request)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(
            request.CartItemId,
            userId
        );

        if (cartItem == null)
            throw new NotFoundException("Sản phẩm không tồn tại trong giỏ hàng.");

        var variant = cartItem.ProductVariant;

        if (variant.Status != ActiveStatus)
            throw new BadRequestException("Biến thể sản phẩm đã ngừng bán.");

        if (variant.Product.Status != ActiveStatus)
            throw new BadRequestException("Sản phẩm đã ngừng bán.");

        if (request.Quantity > variant.StockQuantity)
            throw new BadRequestException("Số lượng cập nhật vượt quá số lượng tồn kho.");

        cartItem.Quantity = request.Quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;
        cartItem.Cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        return await GetCartAsync(userId);
    }


    /// <summary>
    /// Xóa 1 sản phẩm khỏi giỏ hàng. Kiểm tra tồn tại của sản phẩm trong giỏ hàng trước khi xóa.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cartItemId"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task DeleteCartItemAsync(long userId, long cartItemId)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId, userId);

        if (cartItem == null)
            throw new NotFoundException("Sản phẩm không tồn tại trong giỏ hàng.");

        _cartRepository.RemoveCartItem(cartItem);

        await _cartRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Xóa toàn bộ giỏ hàng của người dùng, bao gồm tất cả sản phẩm trong giỏ.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>

    public async Task ClearCartAsync(long userId)
    {
        var cart = await _cartRepository.GetCartDetailByUserIdAsync(userId);

        if (cart == null || !cart.CartItems.Any())
            return;

        _cartRepository.RemoveCartItems(cart.CartItems);

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();
    }


    /// <summary>
    /// Chuyển dữ liệu từ entity Cart sang DTO CartResponse, tính toán giá tiền và tổng tiền cho từng sản phẩm cũng như tổng giỏ hàng.
    /// </summary>
    /// <param name="cart"></param>
    /// <returns></returns>
    private static CartResponse MapToCartResponse(Cart cart)
    {
        var items = cart.CartItems.Select(x =>
        {
            var unitPrice = GetUnitPrice(x.ProductVariant);

            return new CartItemResponse
            {
                CartItemId = x.CartItemId,
                ProductId = x.ProductVariant.ProductId,
                ProductVariantId = x.ProductVariantId,
                ProductName = x.ProductVariant.Product.ProductName,
                Size = x.ProductVariant.SizeValue,
                Color = x.ProductVariant.ColorName,
                Price = unitPrice,
                Quantity = x.Quantity,
                StockQuantity = x.ProductVariant.StockQuantity,
                TotalPrice = x.Quantity * unitPrice
            };
        }).ToList();

        return new CartResponse
        {
            CartId = cart.CartId,
            UserId = cart.UserId,
            Items = items,
            TotalQuantity = items.Sum(x => x.Quantity),
            TotalAmount = items.Sum(x => x.TotalPrice)
        };
    }


    /// <summary>
    /// Xác định giá tiền của sản phẩm dựa trên thứ tự ưu tiên: PriceOverride > SalePrice > BasePrice.
    /// </summary>
    /// <param name="variant">Biến thể sản phẩm cần xác định giá tiền.</param>
    /// <returns>Giá tiền của sản phẩm. </returns>
    private static decimal GetUnitPrice(ProductVariant variant)
    {
        return variant.PriceOverride
            ?? variant.Product.SalePrice
            ?? variant.Product.BasePrice;
    }
}