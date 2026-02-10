using System.ComponentModel.DataAnnotations;

namespace RomBooking.Models.ViewModels;

public class BookingViewModel
{
    public DateTime SelectedDate { get; set; } = DateTime.Today;
    public List<Room> Rooms { get; set; } = new();
    public Dictionary<int, List<Booking>> RoomBookings { get; set; } = new();
}

public class CreateBookingViewModel
{
    [Required(ErrorMessage = "Rom må velges")]
    public int RoomId { get; set; }
    
    [Required(ErrorMessage = "Dato må velges")]
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "Starttid må velges")]
    public TimeSpan StartTime { get; set; }
    
    [Required(ErrorMessage = "Sluttid må velges")]
    public TimeSpan EndTime { get; set; }
    
    [StringLength(500, ErrorMessage = "Notater kan ikke være lengre enn 500 tegn")]
    public string? Notes { get; set; }
    
    public string? RoomName { get; set; }
}

public class MyBookingsViewModel
{
    public List<Booking> UpcomingBookings { get; set; } = new();
    public List<Booking> PastBookings { get; set; } = new();
}
