using Microsoft.AspNetCore.Identity;

namespace ECommerce.WebAPI.Models.Entities;

public class User : BaseEntity
{
    public User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Role = string.Empty;
        Orders = new List<Order>();
    }

    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
}
