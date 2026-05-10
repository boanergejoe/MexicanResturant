using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MexicanRestaurant.Data;
using MexicanRestaurant.Models;
using MexicanRestaurant.Services;
using Microsoft.AspNetCore.Authorization;

namespace MexicanRestaurant.Controllers;

public class MenuController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public MenuController(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index(string category = "All", string search = "")
    {
        var menuItems = await _context.MenuItems
            .Where(m => m.IsAvailable)
            .ToListAsync();

        if (!string.IsNullOrEmpty(category) && category != "All")
        {
            menuItems = menuItems.Where(m => m.Category == category).ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            menuItems = menuItems.Where(m =>
                m.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                m.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                m.Category.Contains(search, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        ViewBag.Categories = new List<string> { "All", "Tacos", "Burritos", "Nachos", "Drinks" };
        ViewBag.SelectedCategory = category;
        ViewBag.SearchTerm = search;

        return View(menuItems);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int menuItemId)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login to add items to cart" });
        }

        await _cartService.AddToCartAsync(userId, menuItemId);
        var cartCount = await _cartService.GetCartItemCountAsync(userId);

        return Json(new { success = true, cartCount = cartCount });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var menuItems = await _context.MenuItems.ToListAsync();
        return View(menuItems);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItem model)
    {
        if (ModelState.IsValid)
        {
            model.CreatedAt = DateTime.Now;
            _context.MenuItems.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Manage");
        }

        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null)
        {
            return NotFound();
        }

        return View(menuItem);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MenuItem model)
    {
        if (ModelState.IsValid)
        {
            _context.MenuItems.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Manage");
        }

        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null)
        {
            return NotFound();
        }

        return View(menuItem);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem != null)
        {
            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Manage");
    }
}