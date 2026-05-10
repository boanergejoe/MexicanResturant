using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MexicanRestaurant.Data;
using MexicanRestaurant.Models;

namespace MexicanRestaurant.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Get statistics
        ViewBag.TotalOrders = await _context.Orders.CountAsync();
        ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => o.Total);
        ViewBag.TotalCustomers = await _context.Users.CountAsync();
        ViewBag.PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");

        // Get recent orders
        var recentOrders = await _context.Orders
            .Include(o => o.OrderDetails)
            .OrderByDescending(o => o.OrderDate)
            .Take(10)
            .ToListAsync();

        return View(recentOrders);
    }

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.MenuItem)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Orders");
    }

    [HttpGet]
    public async Task<IActionResult> MenuItems()
    {
        var menuItems = await _context.MenuItems.ToListAsync();
        return View(menuItems);
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Reports()
    {
        ViewBag.TotalOrders = await _context.Orders.CountAsync();
        ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => o.Total);
        ViewBag.PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
        ViewBag.AverageOrderValue = await _context.Orders.AnyAsync()
            ? await _context.Orders.AverageAsync(o => o.Total)
            : 0m;

        var topItems = await _context.OrderDetails
            .Include(od => od.MenuItem)
            .Where(od => od.MenuItem != null)
            .GroupBy(od => od.MenuItem!.Name)
            .Select(g => new
            {
                Name = g.Key,
                Quantity = g.Sum(od => od.Quantity)
            })
            .OrderByDescending(g => g.Quantity)
            .Take(5)
            .ToListAsync();

        ViewBag.TopItems = topItems;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.ActiveUsers = await _context.Users.CountAsync(u => u.EmailConfirmed);
        ViewBag.PendingActivation = await _context.Users.CountAsync(u => !u.EmailConfirmed);
        ViewBag.TotalMenuItems = await _context.MenuItems.CountAsync();
        ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => o.Total);
        return View();
    }
}