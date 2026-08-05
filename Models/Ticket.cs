namespace EventTicketingApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!;

        public Guid TicketCode { get; set; } = Guid.NewGuid();
        public bool IsCheckedIn { get; set; }
        public DateTime? CheckedInAt { get; set; }
    }
}