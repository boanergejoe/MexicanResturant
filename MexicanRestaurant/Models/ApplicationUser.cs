using Microsoft.AspNetCore.Identity;

namespace MexicanRestaurant.Models;

public class ApplicationUser : IdentityUser
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }
}
