// DTOs/EventDto.cs
namespace EventTicketingApp.DTOs
{
    public record EventDto(
        int Id,
        string Title,
        string Description,
        string VenueName,
        string City,
        DateTime StartDateTime,
        DateTime EndDateTime,
        string ImageUrl,
        List<TicketTypeDto> TicketTypes);
}