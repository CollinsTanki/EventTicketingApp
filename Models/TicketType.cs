using EventTicketingApp.Models;

public class TicketType
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int QuantityAvailable { get; set; }
    public int QuantitySold { get; set; }

    public uint Version { get; set; }   // replaces [Timestamp] byte[] RowVersion
}