using Microsoft.AspNetCore.Mvc;

namespace RomBooking.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Booking");
        }
        
        return RedirectToAction("Login", "Account");
    }
}
