// Controllers/TicketsController.cs
using EventTicketingApp.Data;
using EventTicketingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{ticketCode}/qr")]
        [Authorize]
        public IActionResult GetQrCode(Guid ticketCode)
        {
            var base64 = QrCodeGenerator.GenerateQrCodeBase64(ticketCode);
            return Ok(new { qrCode = base64 });
        }

        [HttpPost("checkin/{ticketCode}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> CheckIn(Guid ticketCode)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketCode == ticketCode);
            if (ticket is null) return NotFound("Invalid ticket.");
            if (ticket.IsCheckedIn) return BadRequest("Ticket already checked in.");

            ticket.IsCheckedIn = true;
            ticket.CheckedInAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Checked in successfully.", checkedInAt = ticket.CheckedInAt });
        }
    }
}