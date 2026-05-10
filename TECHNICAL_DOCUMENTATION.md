# Mexican Restaurant - Technical Documentation

## Architecture Overview

This is a full-stack ASP.NET Core MVC web application built with .NET 10.0, featuring Entity Framework Core for data access, ASP.NET Core Identity for authentication, and Bootstrap 5 for responsive UI.

## Technology Stack

### Backend
- **Framework**: ASP.NET Core MVC 8.0 (.NET 10.0)
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with roles
- **Payment Processing**: Paystack API integration
- **Architecture**: MVC pattern with service layer

### Frontend
- **UI Framework**: Bootstrap 5.3
- **Icons**: Font Awesome 6.4
- **JavaScript**: jQuery 3.7 for AJAX operations
- **Styling**: Custom CSS with responsive design
- **Images**: Unsplash API for dynamic content

### Development Tools
- **IDE**: Visual Studio 2022 / VS Code
- **Build Tool**: dotnet CLI
- **Package Manager**: NuGet
- **Version Control**: Git

## Project Structure

```
MexicanRestaurant/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── CartController.cs
│   ├── HomeController.cs
│   └── MenuController.cs
├── Data/                  # Data Access Layer
│   └── ApplicationDbContext.cs
├── Models/               # Domain Models
│   ├── CartItem.cs
│   ├── MenuItem.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   ├── Payment.cs
│   └── ErrorViewModel.cs
├── Services/             # Business Logic
│   ├── CartService.cs
│   └── PaystackService.cs
├── Views/                # Razor Views
│   ├── Shared/_Layout.cshtml
│   ├── Home/Index.cshtml
│   ├── Menu/Index.cshtml
│   ├── Cart/Index.cshtml
│   ├── Admin/Index.cshtml
│   └── Account/*.cshtml
├── wwwroot/              # Static Assets
│   ├── css/site.css
│   ├── js/site.js
│   └── lib/ (Bootstrap, jQuery)
└── Program.cs            # Application Entry Point
```

## Database Schema

### Core Entities

#### MenuItem
```csharp
public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### Order
```csharp
public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
}
```

#### OrderDetail
```csharp
public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public MenuItem MenuItem { get; set; }
    public Order Order { get; set; }
}
```

#### CartItem
```csharp
public class CartItem
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public MenuItem MenuItem { get; set; }
}
```

## Key Controllers

### MenuController
- **Index**: Displays menu with search and category filtering
- **Search**: Backend search functionality
- **AddToCart**: AJAX endpoint for cart operations

```csharp
public async Task<IActionResult> Index(string category = "All", string search = "")
{
    var query = _context.MenuItems.Where(m => m.IsAvailable);

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(m =>
            m.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            m.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            m.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    if (category != "All")
    {
        query = query.Where(m => m.Category == category);
    }

    var menuItems = await query.ToListAsync();
    ViewBag.SelectedCategory = category;
    ViewBag.SearchTerm = search;

    return View(menuItems);
}
```

### CartController
- **Index**: Display cart contents
- **Add**: Add items to cart (AJAX)
- **Update**: Update item quantities (AJAX)
- **Remove**: Remove items from cart (AJAX)
- **Count**: Get cart item count for badge

### AdminController
- **Index**: Dashboard with metrics
- **Orders**: Order management
- **MenuItems**: CRUD operations for menu
- **Users**: User management

## Services

### CartService
Handles cart operations and business logic:
```csharp
public class CartService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public async Task AddToCartAsync(string userId, int menuItemId, int quantity)
    {
        // Implementation
    }

    public async Task<decimal> GetCartTotalAsync(string userId)
    {
        // Implementation
    }
}
```

### PaystackService
Handles payment processing:
```csharp
public class PaystackService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;

    public async Task<PaymentResponse> InitializePaymentAsync(PaymentRequest request)
    {
        // Implementation
    }
}
```

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MexicanRestaurant;Trusted_Connection=True;"
  },
  "Paystack": {
    "SecretKey": "your_paystack_secret_key",
    "PublicKey": "your_paystack_public_key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Program.cs Setup
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<PaystackService>();
```

## API Endpoints

### Cart Endpoints
- `GET /Cart/Count` - Get cart item count
- `POST /Cart/Add` - Add item to cart
- `POST /Cart/Update` - Update item quantity
- `POST /Cart/Remove` - Remove item from cart

### Menu Endpoints
- `GET /Menu/Index` - Display menu with filters
- `GET /Menu?search=term&category=All` - Search menu

### Admin Endpoints
- `GET /Admin/Index` - Dashboard
- `GET /Admin/Orders` - Order management
- `GET /Admin/MenuItems` - Menu management

## Frontend Architecture

### Layout Structure
- **_Layout.cshtml**: Global layout with header, navigation, footer
- **Responsive Design**: Mobile-first approach with Bootstrap grid
- **AJAX Operations**: jQuery for dynamic cart updates

### Key JavaScript Functions
```javascript
// Update cart count globally
function updateCartCount() {
    $.get('/Cart/Count', function(data) {
        $('.cart-count').text(data.count);
    });
}

// Cart operations
function addToCart(menuItemId) {
    $.post('/Cart/Add', { menuItemId: menuItemId, quantity: 1 }, function() {
        updateCartCount();
    });
}
```

## Security Implementation

### Authentication & Authorization
- ASP.NET Core Identity with role-based access
- Admin role required for admin controllers
- Anti-forgery tokens on forms

### Payment Security
- Paystack secure payment processing
- No sensitive data stored locally
- HTTPS required for production

## Deployment

### Requirements
- .NET 10.0 Runtime
- SQL Server 2019+
- IIS or Kestrel web server

### Build Process
```bash
# Restore packages
dotnet restore

# Build application
dotnet build --configuration Release

# Run migrations
dotnet ef database update

# Publish
dotnet publish --configuration Release --output ./publish
```

### Environment Variables
```bash
# Database
ConnectionStrings__DefaultConnection="Server=prod-server;Database=MexicanRestaurant;User Id=user;Password=password;"

# Paystack
Paystack__SecretKey="sk_live_..."
Paystack__PublicKey="pk_live_..."
```

## Testing

### Unit Tests
```csharp
[Fact]
public async Task MenuController_Index_ReturnsViewWithMenuItems()
{
    // Arrange
    var controller = new MenuController(_context);

    // Act
    var result = await controller.Index();

    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    var model = Assert.IsAssignableFrom<IEnumerable<MenuItem>>(viewResult.Model);
}
```

### Integration Tests
- Test cart operations
- Test payment flow
- Test admin functionality

## Performance Considerations

### Database Optimization
- EF Core query optimization
- Proper indexing on frequently queried columns
- Connection pooling

### Frontend Optimization
- Minified CSS/JS
- Image optimization
- Lazy loading for images

### Caching Strategy
- Output caching for static content
- In-memory caching for frequently accessed data

## Monitoring & Logging

### Application Insights
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Structured Logging
```csharp
_logger.LogInformation("Order {OrderId} created for user {UserId}", order.Id, userId);
```

## Future Enhancements

### Planned Features
- Real-time order tracking
- Push notifications
- Advanced analytics dashboard
- Mobile app companion
- Multi-language support

### Technical Debt
- Add comprehensive unit test coverage
- Implement API versioning
- Add rate limiting
- Implement caching layer

## Contributing

### Code Standards
- Follow C# coding conventions
- Use async/await for I/O operations
- Implement proper error handling
- Add XML documentation comments

### Git Workflow
```bash
# Feature branch workflow
git checkout -b feature/new-feature
# Make changes
git commit -m "Add new feature"
git push origin feature/new-feature
# Create pull request
```

---

*Technical Documentation v1.0*
*Last Updated: December 2024*