using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RomBooking.Data;
using RomBooking.Models;
using RomBooking.Models.ViewModels;
using RomBooking.Services;

namespace RomBooking.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IBookingService _bookingService;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public BookingController(
        ApplicationDbContext context,
        IBookingService bookingService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _bookingService = bookingService;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Index(DateTime? date)
    {
        var selectedDate = date?.Date ?? DateTime.Today;
        var rooms = await _context.Rooms
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync();
        
        var roomBookings = new Dictionary<int, List<Booking>>();
        
        foreach (var room in rooms)
        {
            var bookings = await _bookingService.GetBookingsForRoom(room.Id, selectedDate);
            roomBookings[room.Id] = bookings;
        }
        
        var viewModel = new BookingViewModel
        {
            SelectedDate = selectedDate,
            Rooms = rooms,
            RoomBookings = roomBookings
        };
        
        return View(viewModel);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create(int roomId, DateTime? date, string? time)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null)
        {
            return NotFound();
        }
        
        var model = new CreateBookingViewModel
        {
            RoomId = roomId,
            RoomName = room.Name,
            Date = date ?? DateTime.Today
        };
        
        if (!string.IsNullOrEmpty(time) && TimeSpan.TryParse(time, out var startTime))
        {
            model.StartTime = startTime;
            model.EndTime = startTime.Add(TimeSpan.FromMinutes(30));
        }
        
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var room = await _context.Rooms.FindAsync(model.RoomId);
            model.RoomName = room?.Name;
            return View(model);
        }
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var startDateTime = model.Date.Date.Add(model.StartTime);
        var endDateTime = model.Date.Date.Add(model.EndTime);
        
        var booking = new Booking
        {
            UserId = user.Id,
            RoomId = model.RoomId,
            StartTime = startDateTime,
            EndTime = endDateTime,
            Notes = model.Notes
        };
        
        var success = await _bookingService.CreateBooking(booking);
        
        if (!success)
        {
            ModelState.AddModelError("", "Tidspunktet er ikke tilgjengelig eller ugyldig");
            var room = await _context.Rooms.FindAsync(model.RoomId);
            model.RoomName = room?.Name;
            return View(model);
        }
        
        TempData["Success"] = "Booking opprettet!";
        return RedirectToAction(nameof(Index), new { date = model.Date });
    }
    
    public async Task<IActionResult> MyBookings()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var allBookings = await _bookingService.GetBookingsForUser(user.Id);
        var now = DateTime.Now;
        
        var viewModel = new MyBookingsViewModel
        {
            UpcomingBookings = allBookings.Where(b => b.StartTime >= now).ToList(),
            PastBookings = allBookings.Where(b => b.StartTime < now).ToList()
        };
        
        return View(viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var success = await _bookingService.CancelBooking(id, user.Id);
        
        if (success)
        {
            TempData["Success"] = "Booking kansellert";
        }
        else
        {
            TempData["Error"] = "Kunne ikke kansellere booking";
        }
        
        return RedirectToAction(nameof(MyBookings));
    }
    
    [HttpGet]
    public async Task<IActionResult> CheckAvailability(int roomId, DateTime date, string startTime, string endTime)
    {
        if (!TimeSpan.TryParse(startTime, out var start) || !TimeSpan.TryParse(endTime, out var end))
        {
            return Json(new { available = false, message = "Ugyldig tidsformat" });
        }
        
        var startDateTime = date.Date.Add(start);
        var endDateTime = date.Date.Add(end);
        
        var isAvailable = await _bookingService.IsSlotAvailable(roomId, startDateTime, endDateTime);
        
        return Json(new { available = isAvailable });
    }
}
