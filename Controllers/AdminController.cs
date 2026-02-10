using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RomBooking.Data;
using RomBooking.Models;

namespace RomBooking.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Rooms()
    {
        var rooms = await _context.Rooms
            .OrderBy(r => r.Name)
            .ToListAsync();
        
        return View(rooms);
    }
    
    [HttpGet]
    public IActionResult CreateRoom()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoom(Room model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        _context.Rooms.Add(model);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Rom opprettet!";
        return RedirectToAction(nameof(Rooms));
    }
    
    [HttpGet]
    public async Task<IActionResult> EditRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }
        
        return View(room);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoom(int id, Room model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }
        
        room.Name = model.Name;
        room.Description = model.Description;
        room.IsActive = model.IsActive;
        
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Rom oppdatert!";
        return RedirectToAction(nameof(Rooms));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }
        
        // Sjekk om rommet har fremtidige bookinger
        var hasFutureBookings = await _context.Bookings
            .AnyAsync(b => b.RoomId == id && b.StartTime > DateTime.Now);
        
        if (hasFutureBookings)
        {
            TempData["Error"] = "Kan ikke slette rom med fremtidige bookinger";
            return RedirectToAction(nameof(Rooms));
        }
        
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Rom slettet!";
        return RedirectToAction(nameof(Rooms));
    }
}
