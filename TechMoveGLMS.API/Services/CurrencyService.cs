using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TechMoveGLMS.Shared.Services
{
    //(ExchangeRate-API,2026)(Microsoft,2026)
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyService> _logger;
        private const string API_URL = "https://api.exchangerate-api.com/v4/latest/USD";
        
        private decimal? _cachedRate;
        private DateTime _lastFetch = DateTime.MinValue;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);
        
        public CurrencyService(HttpClient httpClient, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async Task<decimal> GetUsdToZarRateAsync()
        {
            if (_cachedRate.HasValue && DateTime.Now - _lastFetch < _cacheDuration)
            {
                _logger.LogInformation($"Using cached USD/ZAR rate: {_cachedRate.Value}");
                return _cachedRate.Value;
            }
            
            try
            {
                _logger.LogInformation("Fetching fresh USD/ZAR rate from API...");
                var response = await _httpClient.GetAsync(API_URL);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<ExchangeRateResponse>(json);
                    
                    if (data?.Rates != null && data.Rates.TryGetValue("ZAR", out var rate))
                    {
                        _cachedRate = rate;
                        _lastFetch = DateTime.Now;
                        _logger.LogInformation($"USD/ZAR rate fetched: {rate}");
                        return rate;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exchange rate from API");
            }
            
            const decimal FALLBACK_RATE = 18.5m;
            _logger.LogWarning($"Using fallback USD/ZAR rate: {FALLBACK_RATE}");
            return FALLBACK_RATE;
        }
        
        public async Task<decimal> ConvertUsdToZarAsync(decimal usdAmount)
        {
            var rate = await GetUsdToZarRateAsync();
            return usdAmount * rate;
        }
        
        private class ExchangeRateResponse
        {
            public Dictionary<string, decimal> Rates { get; set; } = new();
        }
    }
}
//ExchangeRate-API,2026. Exchange Rate API Documentation.[Online] Available at:
//https://www.exchangerate-api.com/docs/overview

// Microsoft, 2026. HttpClient Class Documentation.[Online] Available at:
//https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-10.0

