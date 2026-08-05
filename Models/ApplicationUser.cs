using Microsoft.AspNetCore.Identity;

namespace EventTicketingApp.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; } = "";
        public List<Order> Orders { get; set; } = new();
    }
}
