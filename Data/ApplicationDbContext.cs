using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RomBooking.Models;

namespace RomBooking.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Konfigurer relasjoner
        builder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Seed initial rooms
        builder.Entity<Room>().HasData(
            new Room { Id = 1, Name = "Rom 1", Description = "Møterom med projektor", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 2, Name = "Rom 2", Description = "Stort møterom", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 3, Name = "Rom 3", Description = "Lite møterom", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 4, Name = "Rom 4", Description = "Konferanserom", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 5, Name = "Rom 5", Description = "Stillerom", IsActive = true, CreatedAt = DateTime.UtcNow }
        );
    }
}
