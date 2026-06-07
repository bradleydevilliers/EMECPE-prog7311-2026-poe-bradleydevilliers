using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TechMoveGLMS.Tests.Integration;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginRequest = new { email = "admin@techmove.com", password = "Admin123!" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!.Token;
    }

    private class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public int? ClientId { get; set; }
    }

    private class RegisterResponse
    {
        public string Message { get; set; } = "";
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var token = await GetAuthTokenAsync();
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var loginRequest = new { email = "admin@techmove.com", password = "wrong" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        var loginRequest = new { email = "nonexistent@test.com", password = "123" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_NewUser_ReturnsOk()
    {
        var uniqueEmail = $"test{Guid.NewGuid()}@test.com";
        var registerRequest = new { email = uniqueEmail, password = "Test123!", clientId = (int?)null };
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.Equal("Registration successful", result!.Message);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var email = $"duplicate{Guid.NewGuid()}@test.com";
        var registerRequest = new { email = email, password = "Test123!", clientId = (int?)null };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}