using System.Net;
using System.Net.Http.Json;
using TechMoveGLMS.Shared.Models.DTOs;

namespace TechMoveGLMS.Tests.Integration;

public class ContractsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContractsControllerTests(CustomWebApplicationFactory factory)
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
    public async Task GetContracts_ReturnsOkAndList()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/contracts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var contracts = await response.Content.ReadFromJsonAsync<List<ContractDto>>();
        Assert.NotNull(contracts);
    }

    [Fact]
    public async Task PostContract_ThenGetById_ReturnsCreated()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var newClient = new { name = "Contract Test Client", contactDetails = "contract@test.com", region = "Test" };
        var clientResponse = await _client.PostAsJsonAsync("/api/clients", newClient);
        clientResponse.EnsureSuccessStatusCode();
        var client = await clientResponse.Content.ReadFromJsonAsync<ClientDto>();
        int clientId = client!.Id;
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(clientId.ToString()), "clientId");
        formData.Add(new StringContent("Premium"), "serviceLevel");
        formData.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd")), "startDate");
        formData.Add(new StringContent(DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")), "endDate");
        var postResponse = await _client.PostAsync("/api/contracts", formData);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var contract = await postResponse.Content.ReadFromJsonAsync<ContractDto>();
        Assert.NotNull(contract);
        Assert.True(contract.Id > 0);
        var getResponse = await _client.GetAsync($"/api/contracts/{contract.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await getResponse.Content.ReadFromJsonAsync<ContractDto>();
        Assert.Equal(contract.Id, retrieved!.Id);
    }

    [Fact]
    public async Task PatchContractStatus_UpdatesStatus()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var newClient = new { name = "Patch Test Client", contactDetails = "patch@test.com", region = "Test" };
        var clientResponse = await _client.PostAsJsonAsync("/api/clients", newClient);
        clientResponse.EnsureSuccessStatusCode();
        var client = await clientResponse.Content.ReadFromJsonAsync<ClientDto>();
        int clientId = client!.Id;
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(clientId.ToString()), "clientId");
        formData.Add(new StringContent("Basic"), "serviceLevel");
        formData.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd")), "startDate");
        formData.Add(new StringContent(DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")), "endDate");
        var postResponse = await _client.PostAsync("/api/contracts", formData);
        var contract = await postResponse.Content.ReadFromJsonAsync<ContractDto>();
        Assert.NotNull(contract);
        var patchContent = new StringContent("\"Active\"", System.Text.Encoding.UTF8, "application/json");
        var patchResponse = await _client.PatchAsync($"/api/contracts/{contract!.Id}/status", patchContent);
        Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/contracts/{contract.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ContractDto>();
        Assert.Equal("Active", updated!.Status);
    }

    [Fact]
    public async Task DeleteContract_RemovesContract()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var newClient = new { name = "Delete Test Client", contactDetails = "delete@test.com", region = "Test" };
        var clientResponse = await _client.PostAsJsonAsync("/api/clients", newClient);
        clientResponse.EnsureSuccessStatusCode();
        var client = await clientResponse.Content.ReadFromJsonAsync<ClientDto>();
        int clientId = client!.Id;
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(clientId.ToString()), "clientId");
        formData.Add(new StringContent("Basic"), "serviceLevel");
        formData.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd")), "startDate");
        formData.Add(new StringContent(DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")), "endDate");
        var postResponse = await _client.PostAsync("/api/contracts", formData);
        var contract = await postResponse.Content.ReadFromJsonAsync<ContractDto>();
        Assert.NotNull(contract);
        var deleteResponse = await _client.DeleteAsync($"/api/contracts/{contract!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/contracts/{contract.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}