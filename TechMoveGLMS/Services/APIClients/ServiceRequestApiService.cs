using System.Text;
using System.Text.Json;
using TechMoveGLMS.Shared.Models.DTOs;
using TechMoveGLMS.Shared.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace TechMoveGLMS.Services.ApiClients;

public class ServiceRequestApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServiceRequestApiService(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        var apiBaseUrl = _config["ApiBaseUrl"] ?? "http://localhost:5212/";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
    }

    private void EnsureBaseAddress()
    {
        var apiBaseUrl = _config["ApiBaseUrl"] ?? "http://localhost:5212/";
        if (_httpClient.BaseAddress == null || _httpClient.BaseAddress.ToString() != apiBaseUrl)
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
    }

    private void AddBearerToken()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<ServiceRequestDto>> GetAllAsync()
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.GetAsync("api/servicerequests");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<ServiceRequestDto>>(json, options) ?? new();
    }

    public async Task<ServiceRequestDto?> GetByIdAsync(int id)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.GetAsync($"api/servicerequests/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<ServiceRequestDto>(json, options);
    }

    public async Task<ServiceRequest> CreateAsync(int contractId, string description, string serviceLevel, decimal distance, bool isPriority)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(contractId.ToString()), "contractId");
        formData.Add(new StringContent(description), "description");
        formData.Add(new StringContent(serviceLevel), "serviceLevel");
        formData.Add(new StringContent(distance.ToString()), "distance");
        formData.Add(new StringContent(isPriority.ToString().ToLower()), "isPriority");
        var response = await _httpClient.PostAsync("api/servicerequests", formData);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ServiceRequest>(json)!;
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var content = new StringContent($"\"{status}\"", Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"api/servicerequests/{id}/status", content);
        response.EnsureSuccessStatusCode();
    }
}
