using Microsoft.EntityFrameworkCore;
using RomBooking.Data;
using RomBooking.Models;

namespace RomBooking.Services;

public interface IBookingService
{
    Task<bool> IsSlotAvailable(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
    Task<List<Booking>> GetBookingsForRoom(int roomId, DateTime date);
    Task<List<Booking>> GetBookingsForUser(string userId);
    Task<Booking?> GetBookingById(int id);
    Task<bool> CreateBooking(Booking booking);
    Task<bool> CancelBooking(int bookingId, string userId);
    bool IsWithinBusinessHours(TimeSpan time);
    bool IsValidBookingDuration(DateTime startTime, DateTime endTime);
}

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly IHolidayService _holidayService;
    private readonly TimeSpan _openingTime = new TimeSpan(8, 0, 0);  // 08:00
    private readonly TimeSpan _closingTime = new TimeSpan(16, 0, 0); // 16:00
    
    public BookingService(ApplicationDbContext context, IHolidayService holidayService)
    {
        _context = context;
        _holidayService = holidayService;
    }
    
    public async Task<bool> IsSlotAvailable(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
    {
        // Sjekk om det er helligdag
        if (_holidayService.IsHoliday(startTime.Date))
        {
            return false;
        }
        
        // Sjekk om det er helg
        if (startTime.DayOfWeek == DayOfWeek.Saturday || startTime.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }
        
        // Sjekk åpningstider
        if (!IsWithinBusinessHours(startTime.TimeOfDay) || !IsWithinBusinessHours(endTime.TimeOfDay))
        {
            return false;
        }
        
        // Sjekk om det finnes overlappende bookinger
        var query = _context.Bookings
            .Where(b => b.RoomId == roomId &&
                        b.StartTime < endTime &&
                        b.EndTime > startTime);
        
        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }
        
        var hasOverlap = await query.AnyAsync();
        
        return !hasOverlap;
    }
    
    public async Task<List<Booking>> GetBookingsForRoom(int roomId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Room)
            .Where(b => b.RoomId == roomId && 
                        b.StartTime >= startOfDay && 
                        b.StartTime < endOfDay)
            .OrderBy(b => b.StartTime)
            .ToListAsync();
    }
    
    public async Task<List<Booking>> GetBookingsForUser(string userId)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();
    }
    
    public async Task<Booking?> GetBookingById(int id)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
    
    public async Task<bool> CreateBooking(Booking booking)
    {
        if (!IsValidBookingDuration(booking.StartTime, booking.EndTime))
        {
            return false;
        }
        
        if (!await IsSlotAvailable(booking.RoomId, booking.StartTime, booking.EndTime))
        {
            return false;
        }
        
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> CancelBooking(int bookingId, string userId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        
        if (booking == null || booking.UserId != userId)
        {
            return false;
        }
        
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public bool IsWithinBusinessHours(TimeSpan time)
    {
        return time >= _openingTime && time <= _closingTime;
    }
    
    public bool IsValidBookingDuration(DateTime startTime, DateTime endTime)
    {
        var duration = endTime - startTime;
        
        // Minimum 30 minutter
        if (duration.TotalMinutes < 30)
        {
            return false;
        }
        
        // Må være i 30-minutters intervaller
        if (duration.TotalMinutes % 30 != 0)
        {
            return false;
        }
        
        return true;
    }
}
