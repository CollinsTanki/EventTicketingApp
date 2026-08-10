// Services/OrderService.cs
using EventTicketingApp.Data;
using EventTicketingApp.DTOs;
using EventTicketingApp.Helpers;
using EventTicketingApp.Models;
using EventTicketingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string? Error, OrderResponseDto? Order)> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var ticketType = await _context.TicketTypes.FindAsync(item.TicketTypeId);
                if (ticketType is null)
                    return (false, $"Ticket type {item.TicketTypeId} not found.", null);

                if (ticketType.QuantitySold + item.Quantity > ticketType.QuantityAvailable)
                    return (false, $"Not enough tickets available for {ticketType.Name}.", null);

                ticketType.QuantitySold += item.Quantity;
                total += ticketType.Price * item.Quantity;

                var orderItem = new OrderItem
                {
                    TicketTypeId = item.TicketTypeId,
                    Quantity = item.Quantity,
                    UnitPrice = ticketType.Price
                };

                // Generate one Ticket per unit purchased
                for (int i = 0; i < item.Quantity; i++)
                {
                    orderItem.Tickets.Add(new Ticket
                    {
                        TicketCode = Guid.NewGuid()
                    });
                }

                order.Items.Add(orderItem);
            }

            order.TotalAmount = total;

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return (false, "Someone just booked one of these tickets — please try again.", null);
            }

            var response = await GetOrderByIdAsync(order.Id, userId);
            return (true, null, response);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Where(o => o.Id == orderId && o.UserId == userId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.TicketType)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Tickets)
                .FirstOrDefaultAsync();

            if (order is null) return null;

            var tickets = order.Items
                .SelectMany(i => i.Tickets.Select(t => new TicketResponseDto(
                    t.Id, t.TicketCode, i.TicketType.Name)))
                .ToList();

            return new OrderResponseDto(
                order.Id,
                order.Status.ToString(),
                order.TotalAmount,
                order.CreatedAt,
                tickets);
        }
    }
}