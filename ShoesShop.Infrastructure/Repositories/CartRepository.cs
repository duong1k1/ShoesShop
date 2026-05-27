using Microsoft.EntityFrameworkCore;
using ShoesShop.Application.Interfaces.Repositories;
using ShoesShop.Domain.Constants;
using ShoesShop.Domain.Entities;
using ShoesShop.Infrastructure.Persistence;

namespace ShoesShop.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Lấy giỏ hàng đang hoạt động của người dùng theo userId. Nếu không có giỏ hàng nào đang hoạt động, trả về null.
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Giỏ hàng đang hoạt động hoặc null nếu không có </returns>
    public async Task<Cart?> GetCartByUserIdAsync(long userId)
    {
        return await _context.Carts
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Status == DomainConstants.CartStatus.Active);
    }


    /// <summary>
    /// Lấy đầy đủ thông tin giỏ hàng của người dùng theo userId, bao gồm các sản phẩm trong giỏ hàng.
    /// Nếu không có giỏ hàng nào đang hoạt động, trả về null.
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Giỏ hàng đang hoạt động với đầy đủ thông tin hoặc null nếu không có</returns>
    public async Task<Cart?> GetCartDetailByUserIdAsync(long userId)
    {
        return await _context.Carts
            .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                    .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Status == DomainConstants.CartStatus.Active);
    }


    /// <summary>
    /// Tạo ra một giỏ hàng mới cho người dùng với userId. Giỏ hàng mới sẽ có trạng thái "Active" và thời gian tạo là thời điểm hiện tại.
    /// Trả về giỏ hàng vừa được tạo. Nếu người dùng đã có một giỏ hàng đang hoạt động, phương thức này sẽ không tạo giỏ hàng mới mà có thể trả về giỏ hàng hiện tại hoặc xử lý theo yêu cầu của ứng dụng (ví dụ: ném ra lỗi hoặc trả về null).
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Giỏ hàng vừa được tạo hoặc giỏ hàng hiện tại nếu đã tồn tại</returns>
    public async Task<Cart> CreateCartAsync(long userId)
    {
        var cart = new Cart
        {
            UserId = userId,
            Status = DomainConstants.CartStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();

        return cart;
    }

    /// <summary>
    /// Tìm biến thể sản phẩm dựa trên productId, size và color. Nếu không tìm thấy biến thể nào phù hợp, trả về null.
    /// </summary>
    /// <param name="productId">ID của sản phẩm</param>
    /// <param name="size">Kích thước của biến thể</param>
    /// <param name="color">Màu sắc của biến thể</param>
    /// <returns>Biến thể sản phẩm phù hợp hoặc null nếu không tìm thấy</returns>

    public async Task<ProductVariant?> GetVariantAsync(
        long productId,
        string size,
        string color)
    {
        return await _context.ProductVariants
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.SizeValue == size &&
                x.ColorName == color);
    }

    /// <summary>
    /// Kiểm tra trong card có variant đó chưa, nếu có rồi thì trả về cart item đó, nếu chưa có thì trả về null
    /// </summary>
    /// <param name="cartId"></param>
    /// <param name="productVariantId"></param>
    /// <returns></returns>
    public async Task<CartItem?> GetCartItemAsync(
        long cartId,
        long productVariantId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(x =>
                x.CartId == cartId &&
                x.ProductVariantId == productVariantId);
    }


    /// <summary>
    /// Lấy cart item theo cartItemId và userId, đảm bảo rằng cart item đó thuộc về giỏ hàng đang hoạt động của người dùng.
    /// Nếu không tìm thấy cart item nào phù hợp, trả về null.
    /// </summary>
    /// <param name="cartItemId">ID của cart item</param>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Cart item phù hợp hoặc null nếu không tìm thấy</returns>
    public async Task<CartItem?> GetCartItemByIdAsync(
        long cartItemId,
        long userId)
    {
        return await _context.CartItems
            .Include(x => x.Cart)
            .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x =>
                x.CartItemId == cartItemId &&
                x.Cart.UserId == userId &&
                x.Cart.Status == "Active");
    }



    public async Task AddCartItemAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
    }

    public void RemoveCartItem(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
    }

    public void RemoveCartItems(IEnumerable<CartItem> cartItems)
    {
        _context.CartItems.RemoveRange(cartItems);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}