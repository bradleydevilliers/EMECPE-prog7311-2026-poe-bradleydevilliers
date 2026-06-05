using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechMoveGLMS.Shared.Data;
using TechMoveGLMS.Shared.Models.Entities;

namespace TechMoveGLMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return Unauthorized("Invalid email or password");

        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));
        if (user.PasswordHash != hash) return Unauthorized("Invalid email or password");

        var token = GenerateJwtToken(user);
        return Ok(new { token, role = user.Role, clientId = user.ClientId });
    }
    public class RegisterRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public int? ClientId { get; set; }
}

[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    // Check if email already exists
    if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        return BadRequest(new { message = "Email already exists" });

    // Hash the password
    using var sha256 = System.Security.Cryptography.SHA256.Create();
    var hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));

    var user = new User
    {
        Email = request.Email,
        PasswordHash = hash,
        Role = "Client",
        ClientId = request.ClientId,
        IsActive = true,
        CreatedAt = DateTime.Now
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Registration successful" });
}

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("ClientId", user.ClientId?.ToString() ?? "")
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}