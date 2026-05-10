using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MexicanRestaurant.Data;
using MexicanRestaurant.Models;
using MexicanRestaurant.Services;

namespace MexicanRestaurant.Controllers;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;
    private readonly PaystackService _paystackService;
    private readonly IConfiguration _configuration;

    public OrderController(
        ApplicationDbContext context,
        CartService cartService,
        PaystackService paystackService,
        IConfiguration configuration)
    {
        _context = context;
        _cartService = cartService;
        _paystackService = paystackService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var cartItems = await _cartService.GetCartItemsAsync(userId);
        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var subtotal = await _cartService.GetCartTotalAsync(userId);
        var tax = subtotal * 0.08m;
        var total = subtotal + tax;

        ViewBag.Subtotal = subtotal;
        ViewBag.Tax = tax;
        ViewBag.Total = total;
        ViewBag.PublicKey = _configuration["Paystack:PublicKey"];

        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> InitializePayment(CheckoutViewModel model)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login" });
        }

        var cartItems = await _cartService.GetCartItemsAsync(userId);
        if (!cartItems.Any())
        {
            return Json(new { success = false, message = "Cart is empty" });
        }

        var subtotal = await _cartService.GetCartTotalAsync(userId);
        var tax = subtotal * 0.08m;
        var total = subtotal + tax;

        // Create pending order
        var order = new Order
        {
            UserId = userId,
            Subtotal = subtotal,
            Tax = tax,
            Total = total,
            Status = "Pending",
            ShippingAddress = model.Address,
            CustomerName = model.FullName,
            CustomerEmail = model.Email
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Add order details
        foreach (var item in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.Id,
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                UnitPrice = item.MenuItem?.Price ?? 0,
                Subtotal = (item.MenuItem?.Price ?? 0) * item.Quantity
            };
            _context.OrderDetails.Add(orderDetail);
        }
        await _context.SaveChangesAsync();

        // Initialize Paystack transaction
        var paystackResponse = await _paystackService.InitializeTransaction(model.Email, total, $"ORDER_{order.Id}");

        if (paystackResponse.Status && paystackResponse.Data != null)
        {
            // Store reference in session
            HttpContext.Session.SetString("PaystackReference", paystackResponse.Data.Reference ?? "");
            HttpContext.Session.SetInt32("OrderId", order.Id);

            return Json(new
            {
                success = true,
                authorizationUrl = paystackResponse.Data.AuthorizationUrl,
                reference = paystackResponse.Data.Reference
            });
        }

        return Json(new { success = false, message = paystackResponse.Message ?? "Payment initialization failed" });
    }

    [HttpGet]
    public async Task<IActionResult> VerifyPayment(string reference)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var verifyResponse = await _paystackService.VerifyTransaction(reference);

        if (verifyResponse.Status && verifyResponse.Data != null && verifyResponse.Data.Status == "success")
        {
            var orderId = HttpContext.Session.GetInt32("OrderId");
            if (orderId.HasValue)
            {
                var order = await _context.Orders.FindAsync(orderId.Value);
                if (order != null)
                {
                    order.Status = "Paid";

                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        TransactionReference = reference,
                        Status = "Success",
                        Amount = order.Total,
                        PaymentMethod = "Card"
                    };
                    _context.Payments.Add(payment);

                    await _context.SaveChangesAsync();

                    // Clear cart after successful payment
                    await _cartService.ClearCartAsync(userId);

                    return RedirectToAction("Success", new { id = order.Id });
                }
            }
        }

        return RedirectToAction("Cancel");
    }

    [HttpGet]
    public async Task<IActionResult> Success(int id)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    public IActionResult Cancel()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyOrders()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.MenuItem)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.MenuItem)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Manage");
    }

    [HttpPost]
    public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var cartItems = await _cartService.GetCartItemsAsync(userId);
        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var subtotal = await _cartService.GetCartTotalAsync(userId);
        var tax = subtotal * 0.08m;
        var total = subtotal + tax;

        // Create pending order
        var order = new Order
        {
            UserId = userId,
            Subtotal = subtotal,
            Tax = tax,
            Total = total,
            Status = "Pending",
            ShippingAddress = model.Address,
            CustomerName = model.FullName,
            CustomerEmail = model.Email
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Add order details
        foreach (var item in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.Id,
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                UnitPrice = item.MenuItem?.Price ?? 0,
                Subtotal = (item.MenuItem?.Price ?? 0) * item.Quantity
            };
            _context.OrderDetails.Add(orderDetail);
        }
        await _context.SaveChangesAsync();

        // Store order ID in session for payment processing
        HttpContext.Session.SetInt32("OrderId", order.Id);
        HttpContext.Session.SetString("CustomerEmail", model.Email);

        // Route to the selected payment method
        if (model.PaymentMethod == "pesapal")
        {
            return RedirectToAction("PesapalCheckout", new { id = order.Id });
        }
        else
        {
            // Default to Paystack
            return RedirectToAction("PaystackCheckout", new { id = order.Id });
        }
    }

    [HttpGet]
    public async Task<IActionResult> PaystackCheckout(int id)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var order = await _context.Orders.FindAsync(id);
        if (order == null || order.UserId != userId)
        {
            return NotFound();
        }

        ViewBag.PublicKey = _configuration["Paystack:PublicKey"];
        return View("Checkout", order);
    }

    [HttpGet]
    public async Task<IActionResult> PesapalCheckout(int id)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var order = await _context.Orders.FindAsync(id);
        if (order == null || order.UserId != userId)
        {
            return NotFound();
        }

        var pesapalStoreUrl = _configuration["Pesapal:StorePageUrl"] ?? "https://store.pesapal.com/mrestaurant";
        ViewBag.PesapalStoreUrl = pesapalStoreUrl;

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> VerifyPesapalPayment(int orderId)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login" });
        }

        var order = await _context.Orders.FindAsync(orderId);
        if (order == null || order.UserId != userId)
        {
            return Json(new { success = false, message = "Order not found" });
        }

        // In a real implementation, you would verify the payment with Pesapal API
        // For now, we'll mark it as paid when the user confirms payment
        order.Status = "Paid";

        var payment = new Payment
        {
            OrderId = order.Id,
            TransactionReference = $"PESAPAL_{DateTime.Now.Ticks}",
            Status = "Success",
            Amount = order.Total,
            PaymentMethod = "Pesapal"
        };
        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();

        // Clear cart after successful payment
        await _cartService.ClearCartAsync(userId);

        return Json(new { success = true, redirectUrl = Url.Action("Success", new { id = order.Id }) });
    }
}

public class CheckoutViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string Expiry { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "paystack";
}