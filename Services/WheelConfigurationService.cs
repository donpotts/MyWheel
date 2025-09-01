using MyWheelApp.Models;
using System.Text.Json;
using Microsoft.JSInterop;

namespace MyWheelApp.Services
{
    public class WheelConfigurationService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly string _configPath = "data/wheel-config.json";
        private readonly string _localStorageKey = "wheelConfiguration";
        private WheelConfiguration? _currentConfiguration;

        public WheelConfigurationService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<WheelConfiguration> LoadConfigurationAsync()
        {
            if (_currentConfiguration != null)
                return _currentConfiguration;

            try
            {
                // First try to load from localStorage
                var localStorageJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _localStorageKey);
                if (!string.IsNullOrEmpty(localStorageJson))
                {
                    var localConfig = JsonSerializer.Deserialize<WheelConfiguration>(localStorageJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (localConfig?.Items?.Count > 0)
                    {
                        _currentConfiguration = localConfig;
                        return localConfig;
                    }
                }

                // Fall back to loading from JSON file
                var json = await _httpClient.GetStringAsync(_configPath);
                var config = JsonSerializer.Deserialize<WheelConfiguration>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _currentConfiguration = config ?? GetDefaultConfiguration();
                return _currentConfiguration;
            }
            catch
            {
                _currentConfiguration = GetDefaultConfiguration();
                return _currentConfiguration;
            }
        }

        public async Task SaveConfigurationAsync(WheelConfiguration configuration)
        {
            _currentConfiguration = configuration;
            var json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _localStorageKey, json);
        }

        public void RefreshConfiguration()
        {
            _currentConfiguration = null;
        }

        private WheelConfiguration GetDefaultConfiguration()
        {
            return new WheelConfiguration
            {
                WheelName = "Recipe Wheel",
                Items = new List<WheelItem>
                {
                    new() { Text = "Pizza Margherita", Color = "#ff6b6b" },
                    new() { Text = "Chicken Curry", Color = "#4ecdc4" },
                    new() { Text = "Beef Tacos", Color = "#45b7d1" },
                    new() { Text = "Pasta Carbonara", Color = "#96ceb4" },
                    new() { Text = "Salmon Teriyaki", Color = "#feca57" },
                    new() { Text = "Vegetable Stir Fry", Color = "#ff9ff3" },
                    new() { Text = "Greek Salad", Color = "#54a0ff" },
                    new() { Text = "Chocolate Cake", Color = "#5f27cd" }
                }
            };
        }
    }
}