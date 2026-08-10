// DTOs/TicketTypeDto.cs
namespace EventTicketingApp.DTOs
{
    public record TicketTypeDto(
        int Id,
        string Name,
        decimal Price,
        int Available);
}