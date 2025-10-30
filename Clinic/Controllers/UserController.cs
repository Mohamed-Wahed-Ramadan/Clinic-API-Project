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
    public class UserController : ControllerBase
    {
        private readonly AppDbContext context;

        public UserController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost("register")]
        public ActionResult Register(UserRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data", errors = ModelState });

            var exists = context.Users.Any(x => x.Name == dto.Name);
            if (exists)
                return BadRequest(new { message = "This name is already taken" });

            var user = new User
            {
                Name = dto.Name,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Role = "User"
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, dto.Password);

            context.Users.Add(user);
            context.SaveChanges();

            return Ok(new
            {
                message = "User registered successfully",
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

        [HttpPost("login")]
        public ActionResult Login(UserLoginDTO login)
        {
            if (login == null || string.IsNullOrEmpty(login.Name) || string.IsNullOrEmpty(login.Password))
                return BadRequest(new { message = "Please provide username and password" });

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data", errors = ModelState });

            var user = context.Users.FirstOrDefault(x => x.Name == login.Name);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, login.Password);

            if (result == PasswordVerificationResult.Success)
            {
                return Ok(new
                {
                    message = "Login successful",
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
            else
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        [HttpGet("profile/{name}")]
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

        [HttpPut("profile/{name}")]
        public ActionResult UpdateUser(string name, UserUpdateDTO newuser)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Please fill all data correctly", errors = ModelState });

            var user = context.Users.FirstOrDefault(x => x.Name == name);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, newuser.OldPassword);

            if (result != PasswordVerificationResult.Success)
                return BadRequest(new { message = "Old password is incorrect" });

            // تحديث البيانات
            if (!string.IsNullOrEmpty(newuser.FullName))
                user.FullName = newuser.FullName;

            if (!string.IsNullOrEmpty(newuser.Phone))
                user.Phone = newuser.Phone;

            if (!string.IsNullOrEmpty(newuser.NewPassword))
                user.Password = hasher.HashPassword(user, newuser.NewPassword);

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

        [HttpDelete("profile/{name}")]
        public ActionResult DeleteUser(string name)
        {
            var user = context.Users.FirstOrDefault(x => x.Name == name);
            if (user == null)
                return NotFound(new { message = "User not found" });

            context.Users.Remove(user);
            context.SaveChanges();

            return Ok(new { message = "User deleted successfully" });
        }

        [HttpGet("orders")]
        public ActionResult GetAllOrders()
        {
            var orders = context.Orders
                .Where(x => x.Status == "Waiting")
                .Include(o => o.User)
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
                .OrderBy(o => o.Number)
                .ToList();

            return Ok(orders);
        }

        [HttpGet("my-orders/{name}")]
        public ActionResult GetAllOrdersForUser(string name)
        {
            var user = context.Users.FirstOrDefault(x => x.Name == name);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var orders = context.Orders
                .Where(x => x.UserId == user.Id)
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
                    o.UserId
                })
                .ToList();

            return Ok(orders);
        }

        [HttpPost("orders")]
        public ActionResult AddOrder([FromBody] Order order)
        {
            try
            {
                // إزالة التحقق من الـ User في ModelState
                ModelState.Remove("User");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Invalid data", errors = errors });
                }

                // التحقق من أن الوصف ليس فارغاً
                if (string.IsNullOrWhiteSpace(order.Description))
                    return BadRequest(new { message = "Description is required" });

                // التحقق من وجود المستخدم
                var userExists = context.Users.Any(u => u.Id == order.UserId);
                if (!userExists)
                    return BadRequest(new { message = "User not found" });

                // إنشاء كائن Order جديد بدلاً من استخدام الذي من الـ Request
                var newOrder = new Order
                {
                    Description = order.Description,
                    Price = order.Price,
                    NextDate = order.NextDate,
                    Status = string.IsNullOrEmpty(order.Status) ? "Waiting" : order.Status,
                    StatusType = order.StatusType,
                    UserId = order.UserId,
                    CreatedDate = DateTime.Now
                };

                // حساب رقم الدور التالي فقط للطلبات قيد الانتظار
                var maxQueueNumber = context.Orders
                    .Where(o => o.Status == "Waiting")
                    .Max(o => (int?)o.Number) ?? 0;

                newOrder.Number = maxQueueNumber + 1;

                // إضافة وحفظ
                context.Orders.Add(newOrder);
                context.SaveChanges();

                return Ok(new
                {
                    message = "Order added successfully",
                    order = new
                    {
                        newOrder.Id,
                        newOrder.Description,
                        newOrder.Number,
                        newOrder.Price,
                        newOrder.CreatedDate,
                        newOrder.NextDate,
                        newOrder.Status,
                        newOrder.StatusType,
                        newOrder.UserId
                    }
                });
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new { message = "Database error while adding order", error = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error adding order", error = ex.Message });
            }
        }

        [HttpPut("orders/{orderId}")]
        public ActionResult UpdateOrder(int orderId, [FromBody] Order order)
        {
            // إزالة التحقق من الـ User في ModelState
            ModelState.Remove("User");

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data", errors = ModelState });

            var existingOrder = context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (existingOrder == null)
                return NotFound(new { message = "Order not found" });

            // التحقق من أن الوصف ليس فارغاً
            if (string.IsNullOrWhiteSpace(order.Description))
                return BadRequest(new { message = "Description is required" });

            try
            {
                // تحديث الحقول
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

        [HttpDelete("orders/{orderId}/{userId}")]
        public ActionResult DeleteOrder(int orderId, int userId)
        {
            var order = context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            // التحقق من أن الطلب يخص المستخدم
            if (order.UserId != userId)
                return Forbid();

            context.Orders.Remove(order);
            context.SaveChanges();

            return Ok(new { message = "Order deleted successfully" });
        }
    }
}