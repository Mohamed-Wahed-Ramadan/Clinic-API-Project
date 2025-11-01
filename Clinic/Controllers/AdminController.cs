using Clinic.DTOs;
using Clinic.Interfaces.Services;
using Clinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var result = await _adminService.GetAllUsersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("users/{name}")]
        public async Task<ActionResult> GetUserByName(string name)
        {
            try
            {
                var result = await _adminService.GetUserByNameAsync(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("users/{name}")]
        public async Task<ActionResult> UpdateUser(string name, UserUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Please fill all data correctly", errors = ModelState });

                var result = await _adminService.UpdateUserAsync(name, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("users/{name}")]
        public async Task<ActionResult> DeleteUser(string name)
        {
            try
            {
                var result = await _adminService.DeleteUserAsync(name);
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
                var result = await _adminService.GetAllOrdersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult> GetOrderById(int orderId)
        {
            try
            {
                var result = await _adminService.GetOrderByIdAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("orders/status/{status}")]
        public async Task<ActionResult> GetOrdersByStatus(string status)
        {
            try
            {
                var result = await _adminService.GetOrdersByStatusAsync(status);
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

                var result = await _adminService.UpdateOrderAsync(orderId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("orders/{orderId}")]
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            try
            {
                var result = await _adminService.DeleteOrderAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult> GetStatistics()
        {
            try
            {
                var result = await _adminService.GetStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}