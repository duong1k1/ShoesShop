using ShoesShop.Application.DTOs.Carts;

namespace ShoesShop.Application.Interfaces.Services;

public interface ICartService
{
    Task<CartResponse> AddToCartAsync(long userId, AddCartItemRequest request);

    Task<CartResponse> GetCartAsync(long userId);

    Task<CartResponse> UpdateQuantityAsync(long userId, UpdateCartItemRequest request);

    Task DeleteCartItemAsync(long userId, long cartItemId);

    Task ClearCartAsync(long userId);
}