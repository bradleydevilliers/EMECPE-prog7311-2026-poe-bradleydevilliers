using System.Text;
using System.Text.Json;
using TechMoveGLMS.Shared.Models.DTOs;
using TechMoveGLMS.Shared.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace TechMoveGLMS.Services.ApiClients;

public class ContractApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContractApiService(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
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

    public async Task<List<ContractDto>> GetAllAsync(string? status = null, int? clientId = null)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var url = "api/contracts";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
        if (clientId.HasValue) queryParams.Add($"clientId={clientId}");
        if (queryParams.Any()) url += "?" + string.Join("&", queryParams);
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<ContractDto>>(json, options) ?? new();
    }

    public async Task<ContractDto?> GetByIdAsync(int id)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.GetAsync($"api/contracts/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<ContractDto>(json, options);
    }

    public async Task<Contract> CreateAsync(int clientId, string serviceLevel, DateTime startDate, DateTime endDate, IFormFile? signedAgreement)
    {
        EnsureBaseAddress();
        AddBearerToken();
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(clientId.ToString()), "clientId");
        content.Add(new StringContent(serviceLevel), "serviceLevel");
        content.Add(new StringContent(startDate.ToString("yyyy-MM-dd")), "startDate");
        content.Add(new StringContent(endDate.ToString("yyyy-MM-dd")), "endDate");
        if (signedAgreement != null)
        {
            var streamContent = new StreamContent(signedAgreement.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(signedAgreement.ContentType);
            content.Add(streamContent, "signedAgreement", signedAgreement.FileName);
        }
        var response = await _httpClient.PostAsync("api/contracts", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Contract>(json)!;
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var content = new StringContent($"\"{status}\"", Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync($"api/contracts/{id}/status", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.DeleteAsync($"api/contracts/{id}");
        response.EnsureSuccessStatusCode();
    }
}
