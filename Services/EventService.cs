// Services/EventService.cs
using EventTicketingApp.Data;
using EventTicketingApp.DTOs;
using EventTicketingApp.Models;
using EventTicketingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingApp.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventDto>> GetPublishedEventsAsync()
        {
            return await _context.Events
                .Where(e => e.Status == EventStatus.Published)
                .Include(e => e.Venue)
                .Include(e => e.TicketTypes)
                .Select(e => new EventDto(
                    e.Id,
                    e.Title,
                    e.Description,
                    e.Venue.Name,
                    e.Venue.City,
                    e.StartDateTime,
                    e.EndDateTime,
                    e.ImageUrl,
                    e.TicketTypes.Select(t => new TicketTypeDto(
                        t.Id,
                        t.Name,
                        t.Price,
                        t.QuantityAvailable - t.QuantitySold))
                        .ToList()))
                .ToListAsync();
        }

        public async Task<EventDto?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Where(e => e.Id == id)
                .Include(e => e.Venue)
                .Include(e => e.TicketTypes)
                .Select(e => new EventDto(
                    e.Id,
                    e.Title,
                    e.Description,
                    e.Venue.Name,
                    e.Venue.City,
                    e.StartDateTime,
                    e.EndDateTime,
                    e.ImageUrl,
                    e.TicketTypes.Select(t => new TicketTypeDto(
                        t.Id,
                        t.Name,
                        t.Price,
                        t.QuantityAvailable - t.QuantitySold))
                        .ToList()))
                .FirstOrDefaultAsync();
        }
    }
}