using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MexicanRestaurant.Models;

namespace MexicanRestaurant.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // MenuItem configuration
        builder.Entity<MenuItem>(entity =>
        {
            entity.HasIndex(e => e.Category);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        // CartItem configuration
        builder.Entity<CartItem>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasOne(e => e.MenuItem)
                .WithMany()
                .HasForeignKey(e => e.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Order configuration
        builder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.OrderDate);
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.Tax).HasPrecision(18, 2);
            entity.Property(e => e.Total).HasPrecision(18, 2);
        });

        // OrderDetail configuration
        builder.Entity<OrderDetail>(entity =>
        {
            entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.MenuItem)
                .WithMany()
                .HasForeignKey(e => e.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
        });

        // Payment configuration
        builder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.TransactionReference);
            entity.HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });
    }
}