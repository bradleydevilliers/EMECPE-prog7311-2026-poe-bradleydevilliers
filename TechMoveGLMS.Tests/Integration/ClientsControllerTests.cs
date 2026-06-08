using System.Net;
using System.Net.Http.Json;
using TechMoveGLMS.Shared.Models.DTOs;


// These tests cover GET, POST, PUT, DELETE for clients. They ensure the API returns correct status codes and data.

namespace TechMoveGLMS.Tests.Integration;

public class ClientsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClientsControllerTests(CustomWebApplicationFactory factory)
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

    [Fact]
    public async Task GetClients_ReturnsOkAndNonEmptyList()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/clients");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var clients = await response.Content.ReadFromJsonAsync<List<ClientDto>>();
        Assert.NotNull(clients);
    }

    [Fact]
    public async Task PostClient_ThenGetById_ReturnsCreated()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var newClient = new { name = "Test Client", contactDetails = "test@test.com", region = "Test" };
        var postResponse = await _client.PostAsJsonAsync("/api/clients", newClient);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var created = await postResponse.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        var getResponse = await _client.GetAsync($"/api/clients/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task PutClient_UpdatesClient()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var newClient = new { name = "Before", contactDetails = "before@test.com", region = "Before" };
        var postResponse = await _client.PostAsJsonAsync("/api/clients", newClient);
        var created = await postResponse.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(created);
        var updatedClient = new { id = created!.Id, name = "After", contactDetails = "after@test.com", region = "After" };
        var putResponse = await _client.PutAsJsonAsync($"/api/clients/{created.Id}", updatedClient);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/clients/{created.Id}");
        var retrieved = await getResponse.Content.ReadFromJsonAsync<ClientDto>();
        Assert.Equal("After", retrieved!.Name);
    }

    [Fact]
    public async Task DeleteClient_RemovesClient()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var newClient = new { name = "ToDelete", contactDetails = "delete@test.com", region = "Delete" };
        var postResponse = await _client.PostAsJsonAsync("/api/clients", newClient);
        var created = await postResponse.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(created);
        var deleteResponse = await _client.DeleteAsync($"/api/clients/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/clients/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}

// Microsoft, 2026. xUnit and Moq for unit/integration testing.[Online] 
// Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test
// Moq, 2026. Mocking library for .NET.[Online] Available at: https://github.com/moq/moq