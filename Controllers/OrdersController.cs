// Controllers/OrdersController.cs
using System.Security.Claims;
using EventTicketingApp.DTOs;
using EventTicketingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create(CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var (success, error, order) = await _orderService.CreateOrderAsync(userId, dto);
            return success ? Ok(order) : BadRequest(error);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var order = await _orderService.GetOrderByIdAsync(id, userId);
            return order is null ? NotFound() : Ok(order);
        }
    }
}