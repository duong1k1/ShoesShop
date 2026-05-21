using ShoesShop.Domain.Entities;

namespace ShoesShop.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetCartByUserIdAsync(int userId);

    Task<Cart?> GetCartDetailByUserIdAsync(int userId);

    Task<Cart> CreateCartAsync(int userId);

    Task<ProductVariant?> GetVariantAsync(int productId, string size, string color);

    Task<CartItem?> GetCartItemAsync(int cartId, int productVariantId);

    Task<CartItem?> GetCartItemByIdAsync(int cartItemId, int userId);

    Task AddCartItemAsync(CartItem cartItem);

    void RemoveCartItem(CartItem cartItem);

    void RemoveCartItems(IEnumerable<CartItem> cartItems);

    Task SaveChangesAsync();
}