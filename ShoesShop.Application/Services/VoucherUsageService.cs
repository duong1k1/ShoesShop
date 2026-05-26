using Microsoft.EntityFrameworkCore;
using ShoesShop.Application.Interfaces.Services;
using ShoesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesShop.Application.Services
{
    public class VoucherUsageService : IVoucherUsageService
    {
        private readonly DbContext _context;

        public VoucherUsageService(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VoucherUsage>> GetAllHistoriesAsync()
        {
            return await _context.Set<VoucherUsage>().ToListAsync();
        }

        public async Task<decimal> CheckAndApplyVoucherAsync(long userId, string voucherCode, decimal orderTotalAmount)
        {
            // 1. Kiểm tra xem người dùng này đã từng áp dụng mã này cho đơn hàng nào chưa
            // SỬA TẠI ĐÂY: Đồng bộ chính xác chữ UserId và VoucherCode (Viết hoa chữ cái đầu)
            var hasUsed = await _context.Set<VoucherUsage>()
                .AnyAsync(v => v.UserId == userId && v.VoucherCode == voucherCode);

            if (hasUsed)
            {
                throw new Exception("Bạn đã sử dụng mã giảm giá này rồi!");
            }

            // 2. Logic kiểm tra điều kiện mã giảm giá
            decimal discountAmount = 0;

            if (voucherCode.ToUpper() == "SNEAKER2026")
            {
                discountAmount = orderTotalAmount * 0.1m; // Giảm 10% đơn hàng
            }
            else if (voucherCode.ToUpper() == "REDUCED50K" && orderTotalAmount >= 500000)
            {
                discountAmount = 50000; // Giảm thẳng 50k cho đơn từ 500k
            }
            else
            {
                throw new Exception("Mã giảm giá không tồn tại hoặc không đủ điều kiện áp dụng!");
            }

            // 3. Ghi nhận lịch sử sử dụng voucher vào Database
            var usage = new VoucherUsage
            {
                UserId = userId,
                VoucherCode = voucherCode,
                UsedAt = DateTime.UtcNow
            };

            _context.Set<VoucherUsage>().Add(usage);
            await _context.SaveChangesAsync();

            return discountAmount;
        }
    }
}