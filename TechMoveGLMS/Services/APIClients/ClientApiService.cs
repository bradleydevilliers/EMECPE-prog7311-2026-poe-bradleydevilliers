using System.Text.Json;
using TechMoveGLMS.Shared.Models.DTOs;
using TechMoveGLMS.Shared.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace TechMoveGLMS.Services.ApiClients;

public class ClientApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientApiService(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
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

    public async Task<List<ClientDto>> GetAllAsync()
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.GetAsync("api/clients");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<List<ClientDto>>(json, options) ?? new();
    }

    public async Task<ClientDto?> GetByIdAsync(int id)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.GetAsync($"api/clients/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<ClientDto>(json, options);
    }

    public async Task<Client> CreateAsync(Client client)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.PostAsJsonAsync("api/clients", client);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Client>(json)!;
    }

    public async Task UpdateAsync(Client client)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.PutAsJsonAsync($"api/clients/{client.Id}", client);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        EnsureBaseAddress();
        AddBearerToken();
        var response = await _httpClient.DeleteAsync($"api/clients/{id}");
        response.EnsureSuccessStatusCode();
    }
}
