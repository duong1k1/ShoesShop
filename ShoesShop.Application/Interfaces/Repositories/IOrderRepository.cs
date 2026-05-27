using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShoesShop.Domain.Entities;

namespace ShoesShop.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Cart?> GetActiveCartWithItemsAsync(long userId);

    Task<Order?> GetOrderDetailAsync(long orderId, long userId);

    Task<List<Order>> GetOrdersByUserIdAsync(long userId);

    Task AddOrderAsync(Order order);

    Task<bool> IsOrderCodeExistsAsync(string orderCode);

    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);

    Task<Order?> GetOrderByCodeAsync(string orderCode);

    Task<PaymentTransaction?> GetPaymentTransactionByRequestIdAsync(string requestId);

    Task<PaymentTransaction?> GetPaymentTransactionByOrderCodeAsync(string orderCode);

    Task AddPaymentTransactionAsync(PaymentTransaction transaction);

    Task SaveChangesAsync();
}