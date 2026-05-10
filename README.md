# Mexican Restaurant 🍲🌶️

A full-featured ASP.NET Core MVC web application for online food ordering with admin management capabilities.

## Features

### Customer Features
- 🛒 **Online Ordering**: Browse menu, add to cart, secure checkout
- 🔍 **Search & Filter**: Find items by name, category, or description
- 👤 **User Accounts**: Registration, login, order history
- 💳 **Secure Payments**: Paystack & Pesapal integration for safe transactions
- 📱 **Responsive Design**: Works perfectly on desktop and mobile

### Admin Features
- 📊 **Dashboard**: Order metrics, revenue tracking, customer insights
- 🍽️ **Menu Management**: Add, edit, remove menu items
- 📦 **Order Processing**: Manage order status and fulfillment
- 👥 **User Management**: View and manage customer accounts
- 📈 **Reports**: Business analytics and performance metrics

## Technology Stack

- **Backend**: ASP.NET Core MVC 8.0 (.NET 10.0)
- **Database**: SQLite for development, SQL Server for production
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Font Awesome, jQuery
- **Payments**: 
  - Paystack: https://paystack.com
  - Pesapal: https://store.pesapal.com/mrestaurant
- **Architecture**: MVC with service layer pattern

## Quick Start

### Prerequisites
- .NET 10.0 SDK
- SQL Server (LocalDB or full SQL Server)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/mexican-restaurant.git
   cd mexican-restaurant
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update database**
   ```bash
   dotnet ef database update
   ```

4. **Configure Paystack** (optional for payments)
   - Add your Paystack keys to `appsettings.json`
   - Get keys from [Paystack Dashboard](https://dashboard.paystack.com/)

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the app**
   - Open https://localhost:5001 in your browser
   - Create an admin account or use the default admin login

## Project Structure

```
MexicanRestaurant/
├── Controllers/          # MVC Controllers
├── Data/                # EF Core DbContext
├── Models/              # Domain entities
├── Services/            # Business logic
├── Views/               # Razor views
├── wwwroot/             # Static assets
├── appsettings.json     # Configuration
└── Program.cs           # App startup
```

## Configuration

### Database Connection
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MexicanRestaurant;Trusted_Connection=True;"
  }
}
```

### Paystack Integration
```json
{
  "Paystack": {
    "SecretKey": "sk_live_your_secret_key_here",
    "PublicKey": "pk_live_your_public_key_here"
  }
}
```

## Usage

### For Customers
1. Browse the menu or use search
2. Add items to cart
3. Register/login for checkout
4. Complete payment with Paystack
5. Track order status

### For Administrators
1. Login with admin credentials
2. Access dashboard for overview
3. Manage menu items, orders, and users
4. View reports and analytics

## API Endpoints

### Cart Operations
- `GET /Cart/Count` - Get cart item count
- `POST /Cart/Add` - Add item to cart
- `POST /Cart/Update` - Update quantity
- `POST /Cart/Remove` - Remove item

### Menu Operations
- `GET /Menu/Index` - Browse menu
- `GET /Menu?search=term` - Search menu

### Admin Operations
- `GET /Admin/Index` - Dashboard
- `GET /Admin/Orders` - Order management
- `GET /Admin/MenuItems` - Menu CRUD

## Development

### Running Tests
```bash
dotnet test
```

### Building for Production
```bash
dotnet publish --configuration Release
```

### Database Migrations
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Security

- 🔐 ASP.NET Core Identity for authentication
- 🛡️ Anti-forgery protection on forms
- 💰 Secure payment processing via Paystack
- 🔒 Role-based authorization (Admin/Customer)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

### Code Standards
- Follow C# coding conventions
- Use async/await for I/O operations
- Add XML documentation
- Write unit tests for new features

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

- 📖 [User Guide](USER_GUIDE.md)
- 🛠️ [Technical Documentation](TECHNICAL_DOCUMENTATION.md)
- 🐛 [Issues](https://github.com/yourusername/mexican-restaurant/issues)

## Screenshots

### Homepage
![Homepage](screenshots/homepage.png)

### Menu Page
![Menu](screenshots/menu.png)

### Admin Dashboard
![Dashboard](screenshots/admin-dashboard.png)

---

Made with ❤️ for Mexican food lovers