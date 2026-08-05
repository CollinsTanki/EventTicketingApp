using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventTicketingApp.Models;

namespace EventTicketingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Venue -> Event (one-to-many) ---
            builder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Event -> TicketType (one-to-many) ---
            builder.Entity<TicketType>()
                .HasOne(t => t.Event)
                .WithMany(e => e.TicketTypes)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TicketType>()
                .Property(t => t.Price)
                .HasColumnType("decimal(10,2)");

            builder.Entity<TicketType>()
                .Property(t => t.Version)
                .IsConcurrencyToken();

            // --- ApplicationUser -> Order (one-to-many) ---
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Order -> OrderItem (one-to-many) ---
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- TicketType -> OrderItem (one-to-many) ---
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.TicketType)
                .WithMany()
                .HasForeignKey(oi => oi.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- OrderItem -> Ticket (one-to-many) ---
            builder.Entity<Ticket>()
                .HasOne(t => t.OrderItem)
                .WithMany(oi => oi.Tickets)
                .HasForeignKey(t => t.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}