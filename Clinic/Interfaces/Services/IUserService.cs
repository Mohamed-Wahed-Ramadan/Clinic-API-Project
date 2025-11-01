using Clinic.DTOs;

namespace Clinic.Interfaces.Services
{
    public interface IUserService
    {
        Task<object> GetUserByNameAsync(string name);
        Task<object> UpdateUserAsync(string name, UserUpdateDTO dto);
        Task<object> DeleteUserAsync(string name);
        Task<IEnumerable<object>> GetAllOrdersAsync();
        Task<IEnumerable<object>> GetUserOrdersAsync(string name);
        Task<object> AddOrderAsync(int userId, OrderCreateDTO dto);
        Task<object> UpdateOrderAsync(int orderId, OrderUpdateDTO dto);
        Task<object> DeleteOrderAsync(int orderId, int userId);
    }
}