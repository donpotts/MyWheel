using MyWheelApp.Models;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace MyWheelApp.Services
{
    public class DailyCodeService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;
        private readonly string _deviceIdKey = "deviceId";
        private readonly string _lastAccessKey = "lastCodeAccess";

        public DailyCodeService(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Validates a code with the server API and marks local usage on success.
        /// When server returns a token, store it and set Authorization header.
        /// </summary>
        public async Task<DailyCodeResponse> ValidateCodeAsync(string inputCode)
        {
            if (string.IsNullOrWhiteSpace(inputCode))
            {
                return new DailyCodeResponse
                {
                    IsValid = false,
                    Message = "Please enter a code."
                };
            }

            try
            {
                var deviceId = await GetDeviceIdAsync();
                var req = new { Code = inputCode.Trim().ToUpper(), DeviceId = deviceId };
                var resp = await _httpClient.PostAsJsonAsync("api/code/validate", req);

                var body = await resp.Content.ReadFromJsonAsync<ValidateCodeResponseDto?>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resp.IsSuccessStatusCode && body is not null && body.IsValid)
                {
                    // If server provided a JWT token, store it and set HttpClient Authorization
                    if (!string.IsNullOrEmpty(body.Token))
                    {
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "apiToken", body.Token);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.Token);
                    }

                    // Mark local usage
                    await MarkCodeAsUsedAsync(DateTime.Today);

                    return new DailyCodeResponse
                    {
                        IsValid = true,
                        Message = body.Message ?? "Access granted!",
                        ExpiresAt = body.ExpiresAt,
                        RemainingUses = body.RemainingUses
                    };
                }

                return new DailyCodeResponse
                {
                    IsValid = false,
                    Message = body?.Message ?? "Invalid code."
                };
            }
            catch (Exception ex)
            {
                return new DailyCodeResponse
                {
                    IsValid = false,
                    Message = $"Validation failed: {ex.Message}"
                };
            }
        }

        // New helper to restore token from localStorage into HttpClient
        public async Task RestoreTokenFromStorageAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "apiToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>
        /// Checks if user has valid access for today based on local record
        /// </summary>
        public async Task<bool> HasValidAccessAsync()
        {
            try
            {
                var lastAccess = await GetLastAccessAsync();
                if (!lastAccess.HasValue)
                {
                    return false;
                }

                var today = DateTime.Today;
                var now = DateTime.Now;

                var accessGrantedToday = lastAccess.Value.Date == today;
                var notExpiredYet = now < today.AddDays(1);
                var withinTimeLimit = (now - lastAccess.Value).TotalHours < 24;

                var hasAccess = accessGrantedToday && notExpiredYet && withinTimeLimit;

                return hasAccess;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DateTime?> GetAccessExpirationAsync()
        {
            var lastAccess = await GetLastAccessAsync();
            if (!lastAccess.HasValue) return null;

            var today = DateTime.Today;
            if (lastAccess.Value.Date != today) return null;

            return today.AddHours(24);
        }

        public async Task ClearAccessAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _lastAccessKey);
        }

        public async Task<string> GetDeviceIdAsync()
        {
            try
            {
                var deviceId = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _deviceIdKey);
                if (string.IsNullOrEmpty(deviceId))
                {
                    deviceId = Guid.NewGuid().ToString();
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _deviceIdKey, deviceId);
                }
                return deviceId;
            }
            catch
            {
                return Guid.NewGuid().ToString();
            }
        }

        private async Task<DateTime?> GetLastAccessAsync()
        {
            try
            {
                var lastAccessString = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _lastAccessKey);
                if (!string.IsNullOrEmpty(lastAccessString) && DateTime.TryParse(lastAccessString, out var lastAccess))
                {
                    return lastAccess;
                }
            }
            catch
            {
            }
            return null;
        }

        private async Task MarkCodeAsUsedAsync(DateTime date)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _lastAccessKey, DateTime.Now.ToString("O"));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Get today's code from the server (for admin/debug purposes)
        /// </summary>
        public async Task<string> GetTodaysCodeAsync()
        {
            try
            {
                var resp = await _httpClient.GetFromJsonAsync<TodayCodeDto>("api/code/today");
                return resp?.Code ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Generate codes for a date range via the server (admin)
        /// </summary>
        public async Task<List<(DateTime Date, string Code)>> GenerateCodesForDateRangeAsync(DateTime startDate, int days)
        {
            try
            {
                var req = new { StartDate = startDate, Days = days };
                var resp = await _httpClient.PostAsJsonAsync("api/code/generate", req);
                var list = await resp.Content.ReadFromJsonAsync<List<CodePairDto>>();
                if (list == null) return new List<(DateTime, string)>();
                return list.Select(x => (x.Date, x.Code)).ToList();
            }
            catch
            {
                return new List<(DateTime, string)>();
            }
        }

        // DTOs for server endpoints
        private class TodayCodeDto
        {
            public string Code { get; set; }
        }

        private class CodePairDto
        {
            public DateTime Date { get; set; }
            public string Code { get; set; } = string.Empty;
        }

        // Local DTO matching server ValidateCodeResponse
        private class ValidateCodeResponseDto
        {
            public bool IsValid { get; set; }
            public string? Message { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public int RemainingUses { get; set; }
            public string? Token { get; set; } // Added token property
        }
    }
}