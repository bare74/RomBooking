using Microsoft.AspNetCore.Identity;

namespace RomBooking.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
