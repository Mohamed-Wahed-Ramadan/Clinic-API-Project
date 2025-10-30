using Clinic.Data;
using Clinic.DTOs;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext context;

        public AdminController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("users")]
        public ActionResult GetAllUsers()
        {
            var users = context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.FullName,
                    u.Phone,
                    u.Role
                })
                .ToList();

            if (!users.Any())
                return Ok(new List<object>());

            return Ok(users);
        }

        [HttpGet("users/{name}")]
        public ActionResult GetUserByName(string name)
        {
            var user = context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.FullName,
                    u.Phone,
                    u.Role
                })
                .FirstOrDefault(x => x.Name == name);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpPut("users/{name}")]
        public ActionResult UpdateUser(string name, UserUpdateDTO newuser)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Please fill all data correctly", errors = ModelState });

            var user = context.Users.FirstOrDefault(x => x.Name == name);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // تحديث البيانات
            if (!string.IsNullOrEmpty(newuser.FullName))
                user.FullName = newuser.FullName;

            if (!string.IsNullOrEmpty(newuser.Phone))
                user.Phone = newuser.Phone;

            if (!string.IsNullOrEmpty(newuser.Role))
                user.Role = newuser.Role;

            // تحديث كلمة المرور إذا تم توفيرها
            if (!string.IsNullOrEmpty(newuser.NewPassword))
            {
                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, newuser.NewPassword);
            }

            context.Users.Update(user);
            context.SaveChanges();

            return Ok(new
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
            });
        }

        [HttpDelete("users/{name}")]
        public ActionResult DeleteUser(string name)
        {
            var user = context.Users.FirstOrDefault(x => x.Name == name);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // حذف جميع الطلبات المرتبطة بالمستخدم أولاً
            var userOrders = context.Orders.Where(o => o.UserId == user.Id);
            context.Orders.RemoveRange(userOrders);

            context.Users.Remove(user);
            context.SaveChanges();

            return Ok(new { message = "User and related orders deleted successfully" });
        }

        [HttpGet("orders")]
        public ActionResult GetAllOrders()
        {
            var orders = context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedDate)
                .Select(o => new
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
                })
                .ToList();

            return Ok(orders);
        }

        [HttpGet("orders/{orderId}")]
        public ActionResult GetOrderById(int orderId)
        {
            var order = context.Orders
                .Include(o => o.User)
                .Where(o => o.Id == orderId)
                .Select(o => new
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
                })
                .FirstOrDefault();

            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }

        [HttpGet("orders/status/{status}")]
        public ActionResult GetOrdersByStatus(string status)
        {
            var orders = context.Orders
                .Include(o => o.User)
                .Where(o => o.Status == status)
                .OrderBy(o => o.Number)
                .Select(o => new
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
                })
                .ToList();

            return Ok(orders);
        }

        [HttpPut("orders/{orderId}")]
        public ActionResult UpdateOrder(int orderId, [FromBody] Order order)
        {
            // إزالة الـ User من ModelState validation
            ModelState.Remove("User");

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data", errors = ModelState });

            var existingOrder = context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (existingOrder == null)
                return NotFound(new { message = "Order not found" });

            try
            {
                // تحديث الحقول المقدمة فقط
                if (!string.IsNullOrEmpty(order.Description))
                    existingOrder.Description = order.Description;

                if (order.Price.HasValue)
                    existingOrder.Price = order.Price;

                if (order.NextDate.HasValue)
                    existingOrder.NextDate = order.NextDate;

                if (!string.IsNullOrEmpty(order.Status))
                    existingOrder.Status = order.Status;

                if (!string.IsNullOrEmpty(order.StatusType))
                    existingOrder.StatusType = order.StatusType;

                context.SaveChanges();

                return Ok(new
                {
                    message = "Order updated successfully",
                    order = new
                    {
                        existingOrder.Id,
                        existingOrder.Description,
                        existingOrder.Number,
                        existingOrder.Price,
                        existingOrder.CreatedDate,
                        existingOrder.NextDate,
                        existingOrder.Status,
                        existingOrder.StatusType,
                        existingOrder.UserId
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error updating order", error = ex.Message });
            }
        }

        [HttpDelete("orders/{orderId}")]
        public ActionResult DeleteOrder(int orderId)
        {
            var order = context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            context.Orders.Remove(order);
            context.SaveChanges();

            return Ok(new { message = "Order deleted successfully" });
        }

        [HttpGet("statistics")]
        public ActionResult GetStatistics()
        {
            var totalUsers = context.Users.Count();
            var totalOrders = context.Orders.Count();
            var waitingOrders = context.Orders.Count(o => o.Status == "Waiting");
            var completedOrders = context.Orders.Count(o => o.Status == "Completed");
            var totalRevenue = context.Orders
                .Where(o => o.Price.HasValue)
                .Sum(o => o.Price.Value);

            return Ok(new
            {
                totalUsers,
                totalOrders,
                waitingOrders,
                completedOrders,
                totalRevenue
            });
        }
    }
}