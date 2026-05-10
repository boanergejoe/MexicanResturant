using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MexicanRestaurant.Data;
using MexicanRestaurant.Models;
using MexicanRestaurant.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add application services
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<PaystackService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Apply any pending migrations so the local database schema matches the current model.
    try
    {
        context.Database.Migrate();
    }
    catch
    {
        // If a stale development database was created outside of migrations, recreate it here.
        if (app.Environment.IsDevelopment())
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }
        else
        {
            throw;
        }
    }

    // Create roles
    if (!roleManager.RoleExistsAsync("Admin").Result)
    {
        roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
    }
    if (!roleManager.RoleExistsAsync("Customer").Result)
    {
        roleManager.CreateAsync(new IdentityRole("Customer")).Wait();
    }

    // Create default admin
    var adminEmail = "3antos1@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            RegistrationDate = DateTime.UtcNow,
            LastLoginDate = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, "Loku@71santo");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Seed menu items if empty
    if (!context.MenuItems.Any())
    {
        var menuItems = new List<MenuItem>
        {
            // Tacos
            new MenuItem { Name = "Carne Asada Tacos", Description = "Grilled steak with onions and cilantro", Price = 12.99m, Category = "Tacos", ImageUrl = "https://images.unsplash.com/photo-1551504734-5ee1c4a1479b?w=400" },
            new MenuItem { Name = "Chicken Tacos", Description = "Seasoned chicken with salsa verde", Price = 10.99m, Category = "Tacos", ImageUrl = "https://images.unsplash.com/photo-1565299585323-38d6b0865b47?w=400" },
            new MenuItem { Name = "Carnitas Tacos", Description = "Slow-cooked pulled pork", Price = 11.99m, Category = "Tacos", ImageUrl = "https://images.unsplash.com/photo-1599974579688-8dbdd335c77f?w=400" },
            new MenuItem { Name = "Fish Tacos", Description = "Crispy fish with cabbage slaw", Price = 13.99m, Category = "Tacos", ImageUrl = "https://images.unsplash.com/photo-1512838243191-e81e8f66f1fd?w=400" },
            
            // Burritos
            new MenuItem { Name = "Chicken Burrito", Description = "Large burrito with chicken, rice, beans", Price = 14.99m, Category = "Burritos", ImageUrl = "https://images.unsplash.com/photo-1626700051175-6818013e1d4f?w=400" },
            new MenuItem { Name = "Beef Burrito", Description = "Hearty beef burrito with all toppings", Price = 15.99m, Category = "Burritos", ImageUrl = "https://images.unsplash.com/photo-1599974579688-8dbdd335c77f?w=400" },
            new MenuItem { Name = "Veggie Burrito", Description = "Grilled vegetables with guacamole", Price = 12.99m, Category = "Burritos", ImageUrl = "https://images.unsplash.com/photo-1626700051175-6818013e1d4f?w=400" },
            new MenuItem { Name = "Shrimp Burrito", Description = "Fresh shrimp with pico de gallo", Price = 16.99m, Category = "Burritos", ImageUrl = "https://images.unsplash.com/photo-1603133872878-684f208fb84b?w=400" },
            
            // Nachos
            new MenuItem { Name = "Classic Nachos", Description = "Chips with cheese, beans, jalapeños", Price = 9.99m, Category = "Nachos", ImageUrl = "https://images.unsplash.com/photo-1513456852971-30c0b8199d4d?w=400" },
            new MenuItem { Name = "Supreme Nachos", Description = "Loaded with all toppings", Price = 13.99m, Category = "Nachos", ImageUrl = "https://images.unsplash.com/photo-1513456852971-30c0b8199d4d?w=400" },
            new MenuItem { Name = "Chicken Nachos", Description = "Topped with grilled chicken", Price = 12.99m, Category = "Nachos", ImageUrl = "https://images.unsplash.com/photo-1599974579688-8dbdd335c77f?w=400" },
            new MenuItem { Name = "Beef Nachos", Description = "Seasoned beef with all fixings", Price = 12.99m, Category = "Nachos", ImageUrl = "https://images.unsplash.com/photo-1513456852971-30c0b8199d4d?w=400" },
            
            // Drinks
            new MenuItem { Name = "Horchata", Description = "Traditional rice milk drink", Price = 3.99m, Category = "Drinks", ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400" },
            new MenuItem { Name = "Jarritos", Description = "Authentic Mexican soda", Price = 2.99m, Category = "Drinks", ImageUrl = "https://images.unsplash.com/photo-1625772299848-391b6a87d7b3?w=400" },
            new MenuItem { Name = "Agua de Jamaica", Description = "Hibiscus iced tea", Price = 3.49m, Category = "Drinks", ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400" },
            new MenuItem { Name = "Margarita", Description = "Classic lime margarita", Price = 8.99m, Category = "Drinks", ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400" }
        };

        context.MenuItems.AddRange(menuItems);
        context.SaveChanges();
    }
}

app.Run();
