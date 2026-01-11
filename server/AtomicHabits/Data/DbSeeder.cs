using AtomicHabits.Models;
using AtomicHabits.Service;
using Microsoft.EntityFrameworkCore;

namespace AtomicHabits.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAsync(AppDbContext context)
        {
            var roles = new[] { "Admin", "User" };

            foreach (var roleName in roles)
            {
                var roleExists = await context.Roles.AnyAsync(r => r.Name == roleName);

                if (!roleExists)
                {
                    context.Roles.Add(new Role { Name = roleName });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
