using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models.Entities;
using System.Security.Cryptography;
using System.Text;
//(Microsoft,2026)
namespace TechMoveGLMS.Shared.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        // Get current logged-in user
        public User? GetCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null) return null;
            
            return _context.Users
                .Include(u => u.Client)
                .FirstOrDefault(u => u.Id == userId);
        }
        
        // Check if current user is Admin
        public bool IsAdmin()
        {
            var user = GetCurrentUser();
            return user?.Role == "Admin";
        }
        
        // Check if user can access a specific client's data
        public bool CanAccessClient(int clientId)
        {
            var user = GetCurrentUser();
            if (user == null) return false;
            if (user.Role == "Admin") return true;
            
            return user.ClientId == clientId;
        }
        
        // Check if user can access a specific contract
        public bool CanAccessContract(int contractId)
        {
            var user = GetCurrentUser();
            if (user == null) return false;
            if (user.Role == "Admin") return true;
            
            var contract = _context.Contracts.Find(contractId);
            return contract?.ClientId == user.ClientId;
        }
        
        // Login method
        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
                
            if (user == null) return false;
            
            var hashedPassword = HashPassword(password);
            if (user.PasswordHash != hashedPassword) return false;
            
            _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", user.Id);
            _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role);
            
            return true;
        }
        
        // Logout method
        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }
        
        // Register new user
        public async Task<User> RegisterAsync(string email, string password, string role = "Client", int? clientId = null)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role,
                ClientId = clientId
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return user;
        }
        
        // Hash password
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        
        // Create default admin if none exists
        public async Task EnsureAdminExistsAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Role == "Admin"))
            {
                await RegisterAsync("admin@techmove.com", "Admin123!", "Admin");
            }
        }
    }
}
// Microsoft, 2026. Session and State Management in ASP.NET Core.[Online] Available at:
//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-10.0