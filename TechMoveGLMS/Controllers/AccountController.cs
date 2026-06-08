using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Services.ApiClients;
using TechMoveGLMS.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;


// I changed this controller to call the API instead of using the database directly.
// The Register method now fetches clients via ClientApiService, and Login stores the JWT token in session.

namespace TechMoveGLMS.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ClientApiService _clientApi;
    public AccountController(HttpClient httpClient, IConfiguration config, ClientApiService clientApi)
    {
        _httpClient = httpClient;
        _config = config;
             _clientApi = clientApi;
        _httpClient.BaseAddress = new Uri(_config["ApiBaseUrl"] ?? "http://localhost:5212/");
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            HttpContext.Session.SetString("JwtToken", result!.Token);
            HttpContext.Session.SetString("UserRole", result.Role);
            if (result.ClientId.HasValue)
                HttpContext.Session.SetInt32("ClientId", result.ClientId.Value);
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError("", "Invalid email or password");
        return View(model);
    }

        public async Task<IActionResult> Register()
    {
        var clients = await _clientApi.GetAllAsync();
        var model = new RegisterViewModel
        {
            Clients = clients.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", model);
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Registration successful. Please login.";
            return RedirectToAction("Login");
        }
        ModelState.AddModelError("", "Registration failed. Email may already exist.");
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();

    private class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public int? ClientId { get; set; }
    }
}

// Microsoft, 2026. ASP.NET Core MVC – HttpClient usage.[Online] 
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests