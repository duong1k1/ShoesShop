using ShoesShop.Domain.Entities;

namespace ShoesShop.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetCartByUserIdAsync(long userId);

    Task<Cart?> GetCartDetailByUserIdAsync(long userId);

    Task<Cart> CreateCartAsync(long userId);

    Task<ProductVariant?> GetVariantAsync(long productId, string size, string color);

    Task<CartItem?> GetCartItemAsync(long cartId, long productVariantId);

    Task<CartItem?> GetCartItemByIdAsync(long cartItemId, long userId);

    Task AddCartItemAsync(CartItem cartItem);

    void RemoveCartItem(CartItem cartItem);

    void RemoveCartItems(IEnumerable<CartItem> cartItems);

    Task SaveChangesAsync(); // lưu thay đổi vào database
}