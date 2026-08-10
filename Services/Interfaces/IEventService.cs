using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketingApp.DTOs;

namespace EventTicketingApp.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDto>> GetPublishedEventsAsync();
        Task<EventDto?> GetByIdAsync(int id);
    }
}
