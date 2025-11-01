using Clinic.DTOs;
using Clinic.Interfaces.Services;
using Clinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public UserController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid data", errors = ModelState });

                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDTO dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password))
                    return BadRequest(new { message = "Please provide username and password" });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid data", errors = ModelState });

                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("profile/{name}")]
        public async Task<ActionResult> GetUserByName(string name)
        {
            try
            {
                var result = await _userService.GetUserByNameAsync(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("profile/{name}")]
        public async Task<ActionResult> UpdateUser(string name, UserUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Please fill all data correctly", errors = ModelState });

                var result = await _userService.UpdateUserAsync(name, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("profile/{name}")]
        public async Task<ActionResult> DeleteUser(string name)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("orders")]
        public async Task<ActionResult> GetAllOrders()
        {
            try
            {
                var result = await _userService.GetAllOrdersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-orders/{name}")]
        public async Task<ActionResult> GetAllOrdersForUser(string name)
        {
            try
            {
                var result = await _userService.GetUserOrdersAsync(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("orders")]
        public async Task<ActionResult> AddOrder([FromBody] Order order)
        {
            try
            {
                ModelState.Remove("User");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Invalid data", errors = errors });
                }

                var dto = new OrderCreateDTO
                {
                    Description = order.Description,
                    Price = order.Price,
                    NextDate = order.NextDate,
                    Status = order.Status,
                    StatusType = order.StatusType
                };

                var result = await _userService.AddOrderAsync(order.UserId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("orders/{orderId}")]
        public async Task<ActionResult> UpdateOrder(int orderId, [FromBody] Order order)
        {
            try
            {
                ModelState.Remove("User");

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid data", errors = ModelState });

                var dto = new OrderUpdateDTO
                {
                    Description = order.Description,
                    Price = order.Price,
                    NextDate = order.NextDate,
                    Status = order.Status,
                    StatusType = order.StatusType
                };

                var result = await _userService.UpdateOrderAsync(orderId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("orders/{orderId}/{userId}")]
        public async Task<ActionResult> DeleteOrder(int orderId, int userId)
        {
            try
            {
                var result = await _userService.DeleteOrderAsync(orderId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unauthorized")
                    return Forbid();
                return NotFound(new { message = ex.Message });
            }
        }
    }
}