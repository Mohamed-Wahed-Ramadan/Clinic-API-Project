using Clinic.DTOs;
using Clinic.Interfaces;
using Clinic.Interfaces.Services;
using Clinic.Models;
using Microsoft.AspNetCore.Identity;

namespace Clinic.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, dto.OldPassword);

            if (result != PasswordVerificationResult.Success)
                throw new Exception("Old password is incorrect");

            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.Phone))
                user.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.NewPassword))
                user.Password = hasher.HashPassword(user, dto.NewPassword);

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

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CompleteAsync();

            return new { message = "User deleted successfully" };
        }

        public async Task<IEnumerable<object>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetWaitingOrdersWithUserAsync();

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

        public async Task<IEnumerable<object>> GetUserOrdersAsync(string name)
        {
            var user = await _unitOfWork.Users.GetByNameAsync(name);
            if (user == null)
                throw new Exception("User not found");

            var orders = await _unitOfWork.Orders.GetUserOrdersAsync(user.Id);

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
                o.UserId
            });
        }

        public async Task<object> AddOrderAsync(int userId, OrderCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new Exception("Description is required");

            var userExists = await _unitOfWork.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                throw new Exception("User not found");

            var maxQueueNumber = await _unitOfWork.Orders.GetMaxQueueNumberForWaitingAsync();

            var order = new Order
            {
                Description = dto.Description,
                Price = dto.Price,
                NextDate = dto.NextDate,
                Status = string.IsNullOrEmpty(dto.Status) ? "Waiting" : dto.Status,
                StatusType = dto.StatusType,
                UserId = userId,
                CreatedDate = DateTime.Now,
                Number = maxQueueNumber + 1
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return new
            {
                message = "Order added successfully",
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

        public async Task<object> UpdateOrderAsync(int orderId, OrderUpdateDTO dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new Exception("Description is required");

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

        public async Task<object> DeleteOrderAsync(int orderId, int userId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (order.UserId != userId)
                throw new Exception("Unauthorized");

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.CompleteAsync();

            return new { message = "Order deleted successfully" };
        }
    }
}