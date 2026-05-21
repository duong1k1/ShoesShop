using ShoesShop.Application.DTOs.Carts;

namespace ShoesShop.Application.Interfaces.Services;

public interface ICartService
{
    Task<CartResponse> AddToCartAsync(int userId, AddCartItemRequest request);

    Task<CartResponse> GetCartAsync(int userId);

    Task<CartResponse> UpdateQuantityAsync(int userId, UpdateCartItemRequest request);

    Task DeleteCartItemAsync(int userId, int cartItemId);

    Task ClearCartAsync(int userId);
}
