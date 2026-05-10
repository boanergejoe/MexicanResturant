# Mexican Restaurant Web Application - User Guide

## Overview

Welcome to the Mexican Restaurant web application! This is a full-featured online ordering system built with ASP.NET Core MVC that allows customers to browse menus, place orders, and manage their accounts while providing administrators with comprehensive management tools.

## Features

### For Customers
- **Browse Menu**: Explore our authentic Mexican dishes with category filtering
- **Search Functionality**: Find specific items by name, description, or category
- **Shopping Cart**: Add items, adjust quantities, and manage your order
- **User Accounts**: Register and login to track your orders
- **Secure Checkout**: Paystack-integrated payment processing
- **Order History**: View your past orders and current order status

### For Administrators
- **Dashboard**: Overview of orders, revenue, and customer metrics
- **Menu Management**: Add, edit, and remove menu items
- **Order Management**: Process and track customer orders
- **User Management**: View and manage customer accounts
- **Reports**: Analytics and business insights

## Getting Started

### First Time Setup
1. Visit the homepage at the root URL
2. Browse our menu or use the search bar
3. Create an account to place orders
4. Add items to your cart and checkout

### Navigation
- **Home**: Featured dishes and restaurant information
- **Menu**: Full menu with search and filtering
- **About**: Learn about our restaurant and story
- **Contact**: Get in touch with us
- **Cart**: View and manage your current order
- **Login/Register**: Access your account

## Using the Application

### Browsing the Menu
1. Click "Menu" in the navigation
2. Use category filters (All, Tacos, Burritos, Nachos, Drinks)
3. Use the search bar to find specific items
4. Click on items to view details

### Adding Items to Cart
1. From the menu page, click "Add to Cart" on any item
2. The cart icon in the header will show your item count
3. Items are added with quantity 1 by default

### Managing Your Cart
1. Click the cart icon in the header
2. View all items in your cart
3. Adjust quantities using + and - buttons
4. Remove items if needed
5. View order total and proceed to checkout

### Checkout Process
1. From the cart page, click "Proceed to Checkout"
2. Fill in your delivery information
3. Review your order details
4. Complete payment through Paystack
5. Receive order confirmation

### Account Management
1. Register with email and password
2. Login to access your account
3. View order history in "My Orders"
4. Update account information as needed

## Administrator Features

### Accessing Admin Panel
1. Login with an admin account
2. Access admin features through the navigation

### Dashboard
- View key metrics: total orders, revenue, customers
- Monitor pending orders
- Access quick actions for menu and order management

### Managing Menu Items
1. Go to "Menu Items" in admin panel
2. Add new items with name, description, price, category
3. Edit existing items
4. Mark items as available/unavailable

### Processing Orders
1. Go to "Orders" in admin panel
2. View all orders with status
3. Update order status (Pending → Processing → Completed)
4. View order details and customer information

### User Management
1. Go to "Users" in admin panel
2. View all registered users
3. Manage user roles (Customer/Admin)
4. View user activity and order history

## Technical Requirements

### Browser Compatibility
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

### Mobile Support
- Fully responsive design
- Optimized for mobile ordering
- Touch-friendly interface

## Security Features

- **Secure Authentication**: ASP.NET Core Identity
- **Payment Security**: Paystack encrypted processing
- **Data Protection**: Secure database connections
- **Role-Based Access**: Admin and customer permissions

## Troubleshooting

### Common Issues

**Can't add items to cart**
- Ensure you're logged in
- Check if item is marked as available
- Try refreshing the page

**Search not working**
- Check spelling and try different keywords
- Use category filters in combination with search

**Payment issues**
- Verify payment information
- Check internet connection
- Contact support if issues persist

**Admin features not accessible**
- Ensure you're logged in with admin account
- Check user role permissions

### Getting Help

If you encounter issues:
1. Check this documentation
2. Contact our support team
3. Check the FAQ section on our website

## Privacy and Terms

- We respect your privacy and protect your data
- Secure payment processing through Paystack
- Order information is stored securely
- See our full privacy policy for details

---

*Last updated: December 2024*
*Version: 1.0*