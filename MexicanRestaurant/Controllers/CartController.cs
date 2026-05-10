using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MexicanRestaurant.Data;
using MexicanRestaurant.Services;

namespace MexicanRestaurant.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public CartController(ApplicationDbContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var cartItems = await _cartService.GetCartItemsAsync(userId);
        var subtotal = await _cartService.GetCartTotalAsync(userId);
        var tax = subtotal * 0.08m; // 8% tax
        var total = subtotal + tax;

        ViewBag.Subtotal = subtotal;
        ViewBag.Tax = tax;
        ViewBag.Total = total;

        return View(cartItems);
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
    public async Task<IActionResult> Count()
    {
        var userId = User.Identity?.Name;
        var count = 0;

        if (!string.IsNullOrEmpty(userId))
        {
            count = await _cartService.GetCartItemCountAsync(userId);
        }

        return Json(new { success = true, count });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login" });
        }

        await _cartService.UpdateQuantityAsync(cartItemId, quantity);
        var cartItems = await _cartService.GetCartItemsAsync(userId);
        var subtotal = await _cartService.GetCartTotalAsync(userId);
        var tax = subtotal * 0.08m;
        var total = subtotal + tax;

        return Json(new
        {
            success = true,
            subtotal = subtotal.ToString("C"),
            tax = tax.ToString("C"),
            total = total.ToString("C"),
            itemCount = cartItems.Sum(c => c.Quantity)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login" });
        }

        await _cartService.RemoveFromCartAsync(cartItemId);
        var cartCount = await _cartService.GetCartItemCountAsync(userId);

        return Json(new { success = true, cartCount = cartCount });
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login" });
        }

        await _cartService.ClearCartAsync(userId);

        return Json(new { success = true });
    }
}