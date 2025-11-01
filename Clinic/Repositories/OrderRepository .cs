using Clinic.Data;
using Clinic.Interfaces.Repositories;
using Clinic.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetAllWithUserAsync()
        {
            return await _dbSet
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order> GetByIdWithUserAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetByStatusWithUserAsync(string status)
        {
            return await _dbSet
                .Include(o => o.User)
                .Where(o => o.Status == status)
                .OrderBy(o => o.Number)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return await _dbSet
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetWaitingOrdersWithUserAsync()
        {
            return await _dbSet
                .Include(o => o.User)
                .Where(o => o.Status == "Waiting")
                .OrderBy(o => o.Number)
                .ToListAsync();
        }

        public async Task<int> GetMaxQueueNumberForWaitingAsync()
        {
            var maxNumber = await _dbSet
                .Where(o => o.Status == "Waiting")
                .MaxAsync(o => (int?)o.Number);

            return maxNumber ?? 0;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbSet
                .Where(o => o.Price.HasValue)
                .SumAsync(o => o.Price.Value);
        }
    }
}