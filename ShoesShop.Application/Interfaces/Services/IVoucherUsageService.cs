using ShoesShop.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Services
{
    public interface IVoucherUsageService
    {
        Task<IEnumerable<VoucherUsage>> GetAllHistoriesAsync();

        // Sửa tên hàm tại đây cho khớp hoàn toàn với file Service thực tế
        Task<decimal> CheckAndApplyVoucherAsync(long userId, string voucherCode, decimal orderTotalAmount);
    }
}