using EventTicketingApp.Models;

namespace EventTicketingApp.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
    }
}