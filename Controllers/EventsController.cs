// Controllers/EventsController.cs
using EventTicketingApp.DTOs;
using EventTicketingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventDto>>> GetAll()
            => Ok(await _eventService.GetPublishedEventsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetById(int id)
        {
            var evt = await _eventService.GetByIdAsync(id);
            return evt is null ? NotFound() : Ok(evt);
        }
    }
}