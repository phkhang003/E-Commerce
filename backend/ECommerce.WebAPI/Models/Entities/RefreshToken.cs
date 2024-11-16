using System.ComponentModel.DataAnnotations;

namespace ECommerce.WebAPI.Models.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
}