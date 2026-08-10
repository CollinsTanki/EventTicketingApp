using EventTicketingApp.DTOs;

namespace EventTicketingApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<(bool Success, string? Error, OrderResponseDto? Order)> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<OrderResponseDto?> GetOrderByIdAsync(int orderId, string userId);
    }
}
