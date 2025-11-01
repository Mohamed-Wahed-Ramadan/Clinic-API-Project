using Clinic.DTOs;

namespace Clinic.Interfaces.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<object>> GetAllUsersAsync();
        Task<object> GetUserByNameAsync(string name);
        Task<object> UpdateUserAsync(string name, UserUpdateDTO dto);
        Task<object> DeleteUserAsync(string name);
        Task<IEnumerable<object>> GetAllOrdersAsync();
        Task<object> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<object>> GetOrdersByStatusAsync(string status);
        Task<object> UpdateOrderAsync(int orderId, OrderUpdateDTO dto);
        Task<object> DeleteOrderAsync(int orderId);
        Task<object> GetStatisticsAsync();
    }
}