// DTOs/OrderResponseDto.cs
namespace EventTicketingApp.DTOs
{
    public record OrderResponseDto(
        int OrderId,
        string Status,
        decimal TotalAmount,
        DateTime CreatedAt,
        List<TicketResponseDto> Tickets);

    public record TicketResponseDto(
        int TicketId,
        Guid TicketCode,
        string TicketTypeName);
}