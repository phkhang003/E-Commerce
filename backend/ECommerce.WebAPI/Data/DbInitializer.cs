using ECommerce.WebAPI.Data;
using ECommerce.WebAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.WebAPI.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Tạo migration nếu chưa có
        await context.Database.MigrateAsync();

        // Tạo admin nếu chưa có
        if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
        {
            var adminUser = new User
            {
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}