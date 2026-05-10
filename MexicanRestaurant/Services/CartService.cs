using Microsoft.EntityFrameworkCore;
using MexicanRestaurant.Data;
using MexicanRestaurant.Models;

namespace MexicanRestaurant.Services;

public class CartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CartItem>> GetCartItemsAsync(string userId)
    {
        return await _context.CartItems
            .Include(c => c.MenuItem)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<CartItem> AddToCartAsync(string userId, int menuItemId, int quantity = 1)
    {
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.MenuItemId == menuItemId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            await _context.SaveChangesAsync();
            return existingItem;
        }

        var cartItem = new CartItem
        {
            UserId = userId,
            MenuItemId = menuItemId,
            Quantity = quantity
        };

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        return cartItem;
    }

    public async Task UpdateQuantityAsync(int cartItemId, int quantity)
    {
        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromCartAsync(int cartItemId)
    {
        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        var cartItems = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetCartTotalAsync(string userId)
    {
        var cartItems = await GetCartItemsAsync(userId);
        return cartItems.Sum(c => (c.MenuItem?.Price ?? 0) * c.Quantity);
    }

    public async Task<int> GetCartItemCountAsync(string userId)
    {
        return await _context.CartItems
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity);
    }
}