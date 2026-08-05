using System.Net.Sockets;

namespace EventTicketingApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int VenueId { get; set; }
        public Venue Venue { get; set; } = null!;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string ImageUrl { get; set; } = "";
        public TicketType Status { get; set; } = null!;
        public List<TicketType> TicketTypes { get; set; } = new();
    }
}
