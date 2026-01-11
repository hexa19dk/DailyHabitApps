using AtomicHabits.Models;
using Microsoft.EntityFrameworkCore;

namespace AtomicHabits.Repositories
{
    public interface IUserRepositories
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<bool> ValidatePasswordAsync(User user, string password);
        Task<bool> IsEmailConfirmedAsync(User user);
        Task<bool> SetPasswordResetTokenAsync(User user, string token);
        Task<User?> GetByResetTokenAsync(string token);
        Task<bool> UpdatePasswordAsync(User user, string newPassword);
    }

    public class UserRepositories : IUserRepositories
    {
        private readonly AppDbContext _context;

        public UserRepositories(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var roleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (roleIds == null || !roleIds.Any())
                return new List<string>();

            // Get role names
            var roles = await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();

            return roles;
        }

        public Task<bool> ValidatePasswordAsync(User user, string password) =>
            Task.FromResult(BCrypt.Net.BCrypt.Verify(password, user.PasswordHash));

        public Task<bool> IsEmailConfirmedAsync(User user) =>
            Task.FromResult(true); // Placeholder for actual email confirmation logic

        public async Task<bool> SetPasswordResetTokenAsync(User user, string token)
        {
            user.PasswordResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<User?> GetByResetTokenAsync(string token) =>
            await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.ResetTokenExpiry > DateTime.UtcNow);

        public async Task<bool> UpdatePasswordAsync(User user, string newPassword)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
