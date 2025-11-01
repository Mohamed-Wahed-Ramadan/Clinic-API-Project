using Clinic.Models;

namespace Clinic.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllWithUserAsync();
        Task<Order> GetByIdWithUserAsync(int orderId);
        Task<IEnumerable<Order>> GetByStatusWithUserAsync(string status);
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<IEnumerable<Order>> GetWaitingOrdersWithUserAsync();
        Task<int> GetMaxQueueNumberForWaitingAsync();
        Task<decimal> GetTotalRevenueAsync();
    }
}