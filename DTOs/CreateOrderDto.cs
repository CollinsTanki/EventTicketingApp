// DTOs/CreateOrderDto.cs
namespace EventTicketingApp.DTOs
{
    public record CreateOrderDto(
        int EventId,
        List<OrderItemDto> Items);

    public record OrderItemDto(
        int TicketTypeId,
        int Quantity);
}