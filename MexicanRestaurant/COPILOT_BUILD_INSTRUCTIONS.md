### Copilot Build Instructions for Mexican Restaurant Web Application

Use these instructions to implement the website exactly as the reference image provided.

#### Project Goal
Build a full ASP.NET Core MVC Mexican Restaurant application with:
- exact UI layout, spacing, colors, typography, card sizes, shadows, and sections from the screenshot
- Bootstrap 5 and custom CSS
- Font Awesome icons
- responsive customer homepage, shopping cart page, auth area, and admin dashboard
- Paystack payment integration, order history, and admin order management
- SQL Server database with EF Core and Identity authentication

#### UI Requirements (match exactly)
- Top left: customer homepage with header logo, nav links, search icon, cart icon, red Login button
- Hero section: taco banner image, dark overlay, large white headline, subtitle, red "Order Now" button
- Popular Dishes: 4 food cards with image, name, red price, description, green "Add to Cart" button
- Top right: shopping cart page with same cart design, item rows, subtotal/tax/total box, red checkout button, payment logos, checkout form fields, and large red payment button
- Bottom left: auth section with Mexican themed background and side-by-side Login and Register cards
- Bottom right: admin dashboard with dark sidebar, white cards, summary metrics, revenue chart, recent orders table
- Keep exact colors, spacing, alignment, component sizes, shadows, and layout positions
- Do NOT change design or rearrange sections

#### Functional Requirements
- Use ASP.NET Core MVC with controllers, views, and layout
- Use EF Core and ApplicationDbContext for MenuItems, CartItems, Orders, OrderDetails, Payments
- Use ASP.NET Core Identity for Customer and Admin roles
- Seed default admin: Email `3antos1@gmail.com`, Password `Loku@71santo`
- Only this admin may create new admin accounts
- Categories: Tacos, Burritos, Nachos, Drinks
- Cart: add/update/remove items, calculate subtotal, tax, total
- Paystack: checkout session, payment verification, success page, cancel page, store transaction reference
- Customer: order history view
- Admin: view all orders, update order status, manage menu, manage users

#### Database Entities
- AspNetUsers
- AspNetRoles
- MenuItems
- CartItems
- Orders
- OrderDetails
- Payments

#### Styling Rules
- Use Bootstrap 5, custom CSS, and Font Awesome
- Red buttons, green add-to-cart buttons, dark sidebar, white cards
- Mexican restaurant themed imagery and typography
- Exact card sizes and layout as image

#### Notes
- Implement the UI exactly; no redesign
- Preserve the reference image’s exact visual structure when the app runs
- Include complete views, controllers, models, services, and CSS for the app
- Fix build issues by importing the correct namespaces and resolving all compilation errors
