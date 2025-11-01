using Clinic.DTOs;
using Clinic.Interfaces;
using Clinic.Interfaces.Services;
using Clinic.Models;
using Microsoft.AspNetCore.Identity;

namespace Clinic.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            return users.Select(u => new
            {
                u.Id,
                u.Name,
                u.FullName,
                u.Phone,
                u.Role
            });
        }

        public async Task<object> GetUserByNameAsync(string name)
        {
            var user = await _unitOfWork.Users.GetByNameAsync(name);
            if (user == null)
                throw new Exception("User not found");

            return new
            {
                user.Id,
                user.Name,
                user.FullName,
                user.Phone,
                user.Role
            };
        }

        public async Task<object> UpdateUserAsync(string name, UserUpdateDTO dto)
        {
            var user = await _unitOfWork.Users.GetByNameAsync(name);
            if (user == null)
                throw new Exception("User not found");

            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.Phone))
                user.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.Role))
                user.Role = dto.Role;

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, dto.NewPassword);
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return new
            {
                message = "User information updated successfully",
                user = new
                {
                    user.Id,
                    user.Name,
                    user.FullName,
                    user.Phone,
                    user.Role
                }
            };
        }

        public async Task<object> DeleteUserAsync(string name)
        {
            var user = await _unitOfWork.Users.GetByNameAsync(name);
            if (user == null)
                throw new Exception("User not found");

            var userOrders = await _unitOfWork.Orders.FindAsync(o => o.UserId == user.Id);
            _unitOfWork.Orders.RemoveRange(userOrders);

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CompleteAsync();

            return new { message = "User and related orders deleted successfully" };
        }

        public async Task<IEnumerable<object>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllWithUserAsync();

            return orders.Select(o => new
            {
                o.Id,
                o.Description,
                o.Number,
                o.Price,
                o.CreatedDate,
                o.NextDate,
                o.Status,
                o.StatusType,
                o.UserId,
                User = new
                {
                    o.User.Id,
                    o.User.Name,
                    o.User.FullName,
                    o.User.Phone
                }
            });
        }

        public async Task<object> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdWithUserAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            return new
            {
                order.Id,
                order.Description,
                order.Number,
                order.Price,
                order.CreatedDate,
                order.NextDate,
                order.Status,
                order.StatusType,
                order.UserId,
                User = new
                {
                    order.User.Id,
                    order.User.Name,
                    order.User.FullName,
                    order.User.Phone
                }
            };
        }

        public async Task<IEnumerable<object>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _unitOfWork.Orders.GetByStatusWithUserAsync(status);

            return orders.Select(o => new
            {
                o.Id,
                o.Description,
                o.Number,
                o.Price,
                o.CreatedDate,
                o.NextDate,
                o.Status,
                o.StatusType,
                o.UserId,
                User = new
                {
                    o.User.Id,
                    o.User.Name,
                    o.User.FullName,
                    o.User.Phone
                }
            });
        }

        public async Task<object> UpdateOrderAsync(int orderId, OrderUpdateDTO dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (!string.IsNullOrEmpty(dto.Description))
                order.Description = dto.Description;

            if (dto.Price.HasValue)
                order.Price = dto.Price;

            if (dto.NextDate.HasValue)
                order.NextDate = dto.NextDate;

            if (!string.IsNullOrEmpty(dto.Status))
                order.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.StatusType))
                order.StatusType = dto.StatusType;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();

            return new
            {
                message = "Order updated successfully",
                order = new
                {
                    order.Id,
                    order.Description,
                    order.Number,
                    order.Price,
                    order.CreatedDate,
                    order.NextDate,
                    order.Status,
                    order.StatusType,
                    order.UserId
                }
            };
        }

        public async Task<object> DeleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();

            return new { message = "Order deleted successfully" };
        }

        public async Task<object> GetStatisticsAsync()
        {
            var totalUsers = await _unitOfWork.Users.CountAsync(u => true);
            var totalOrders = await _unitOfWork.Orders.CountAsync(o => true);
            var waitingOrders = await _unitOfWork.Orders.CountAsync(o => o.Status == "Waiting");
            var completedOrders = await _unitOfWork.Orders.CountAsync(o => o.Status == "Completed");
            var totalRevenue = await _unitOfWork.Orders.GetTotalRevenueAsync();

            return new
            {
                totalUsers,
                totalOrders,
                waitingOrders,
                completedOrders,
                totalRevenue
            };
        }
    }
}