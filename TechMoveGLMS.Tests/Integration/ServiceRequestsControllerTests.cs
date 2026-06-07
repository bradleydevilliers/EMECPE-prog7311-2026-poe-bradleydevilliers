using System.Net;
using System.Net.Http.Json;
using TechMoveGLMS.Shared.Models.DTOs;

namespace TechMoveGLMS.Tests.Integration;

public class ServiceRequestsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ServiceRequestsControllerTests(CustomWebApplicationFactory factory)
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

    private async Task<int> CreateTestContractAsync()
    {
        var newClient = new { name = "SR Test Client", contactDetails = "sr@test.com", region = "Test" };
        var clientResponse = await _client.PostAsJsonAsync("/api/clients", newClient);
        clientResponse.EnsureSuccessStatusCode();
        var client = await clientResponse.Content.ReadFromJsonAsync<ClientDto>();
        int clientId = client!.Id;
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(clientId.ToString()), "clientId");
        formData.Add(new StringContent("Basic"), "serviceLevel");
        formData.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd")), "startDate");
        formData.Add(new StringContent(DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")), "endDate");
        var contractResponse = await _client.PostAsync("/api/contracts", formData);
        contractResponse.EnsureSuccessStatusCode();
        var contract = await contractResponse.Content.ReadFromJsonAsync<ContractDto>();
        return contract!.Id;
    }

    [Fact]
    public async Task GetServiceRequests_ReturnsOkAndList()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/servicerequests");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var requests = await response.Content.ReadFromJsonAsync<List<ServiceRequestDto>>();
        Assert.NotNull(requests);
    }

    [Fact]
    public async Task PostServiceRequest_ThenGetById_ReturnsCreated()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        int contractId = await CreateTestContractAsync();
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(contractId.ToString()), "contractId");
        formData.Add(new StringContent("Test Request"), "description");
        formData.Add(new StringContent("Premium"), "serviceLevel");
        formData.Add(new StringContent("100"), "distance");
        formData.Add(new StringContent("true"), "isPriority");
        var postResponse = await _client.PostAsync("/api/servicerequests", formData);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var request = await postResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();
        Assert.NotNull(request);
        Assert.True(request.Id > 0);
        Assert.Equal("Test Request", request.Description);
        var getResponse = await _client.GetAsync($"/api/servicerequests/{request.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await getResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();
        Assert.Equal(request.Id, retrieved!.Id);
    }

    [Fact]
    public async Task PatchServiceRequestStatus_UpdatesStatus()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        int contractId = await CreateTestContractAsync();
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(contractId.ToString()), "contractId");
        formData.Add(new StringContent("Status Test"), "description");
        formData.Add(new StringContent("Basic"), "serviceLevel");
        var postResponse = await _client.PostAsync("/api/servicerequests", formData);
        var request = await postResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();
        Assert.NotNull(request);
        var patchContent = new StringContent("\"Completed\"", System.Text.Encoding.UTF8, "application/json");
        var patchResponse = await _client.PatchAsync($"/api/servicerequests/{request!.Id}/status", patchContent);
        Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/servicerequests/{request.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();
        Assert.Equal("Completed", updated!.Status);
    }
}