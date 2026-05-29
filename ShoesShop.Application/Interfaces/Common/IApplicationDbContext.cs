using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ShoesShop.Application.Interfaces.Common
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<VoucherUsage> VoucherUsages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
