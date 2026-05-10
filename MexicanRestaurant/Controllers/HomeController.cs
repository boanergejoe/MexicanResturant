using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MexicanRestaurant.Data;
using MexicanRestaurant.Models;

namespace MexicanRestaurant.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var popularItems = _context.MenuItems
            .Where(m => m.IsAvailable)
            .Take(4)
            .ToList();

        return View(popularItems);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
