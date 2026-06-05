using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Shared.Data;
using TechMoveGLMS.Shared.Models.Entities;
using System.Security.Cryptography;
using System.Text;

namespace TechMoveGLMS.API.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public User? GetCurrentUser()
    {
        var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
        if (userId == null) return null;
        return _context.Users.Include(u => u.Client).FirstOrDefault(u => u.Id == userId);
    }

    public bool IsAdmin()
    {
        var user = GetCurrentUser();
        return user?.Role == "Admin";
    }

    public bool CanAccessClient(int clientId)
    {
        var user = GetCurrentUser();
        if (user == null) return false;
        if (user.Role == "Admin") return true;
        return user.ClientId == clientId;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        if (user == null) return false;
        var hashedPassword = HashPassword(password);
        if (user.PasswordHash != hashedPassword) return false;
        _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", user.Id);
        _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role);
        return true;
    }

    public void Logout() => _httpContextAccessor.HttpContext?.Session.Clear();

    public async Task<User> RegisterAsync(string email, string password, string role = "Client", int? clientId = null)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = HashPassword(password),
            Role = role,
            ClientId = clientId,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public async Task EnsureAdminExistsAsync()
    {
        if (!await _context.Users.AnyAsync(u => u.Role == "Admin"))
        {
            await RegisterAsync("admin@techmove.com", "Admin123!", "Admin");
        }
    }
}
