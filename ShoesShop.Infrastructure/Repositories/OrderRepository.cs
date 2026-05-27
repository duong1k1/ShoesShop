using Microsoft.EntityFrameworkCore;
using ShoesShop.Application.Interfaces.Repositories;
using ShoesShop.Domain.Constants;
using ShoesShop.Domain.Entities;
using ShoesShop.Infrastructure.Persistence;

namespace ShoesShop.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetActiveCartWithItemsAsync(long userId)
    {
        return await _context.Carts
            .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariant)
                    .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Status == DomainConstants.CartStatus.Active);
    }

    public async Task<Order?> GetOrderDetailAsync(long orderId, long userId)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x =>
                x.OrderId == orderId &&
                x.UserId == userId &&
                x.DeletedAt == null);
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(long userId)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .Where(x =>
                x.UserId == userId &&
                x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<bool> IsOrderCodeExistsAsync(string orderCode)
    {
        return await _context.Orders.AnyAsync(x => x.OrderCode == orderCode);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = await action();

            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Order?> GetOrderByCodeAsync(string orderCode)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .Include(x => x.PaymentTransactions)
            .FirstOrDefaultAsync(x => x.OrderCode == orderCode);
    }

    public async Task AddPaymentTransactionAsync(PaymentTransaction transaction)
    {
        await _context.PaymentTransactions.AddAsync(transaction);
    }

    public async Task<PaymentTransaction?> GetPaymentTransactionByRequestIdAsync(string requestId)
    {
        return await _context.PaymentTransactions
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.RequestId == requestId);
    }

    public async Task<PaymentTransaction?> GetPaymentTransactionByOrderCodeAsync(string orderCode)
    {
        return await _context.PaymentTransactions
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.Order.OrderCode == orderCode);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}