using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using MyWheelApp;
using MyWheelApp.Services;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Determine API base address from configuration (wwwroot/appsettings*.json) or fall back to host base
var apiBase = builder.Configuration["ApiBaseUrl"]; 
var baseAddress = string.IsNullOrWhiteSpace(apiBase) ? builder.HostEnvironment.BaseAddress : apiBase;
if (!baseAddress.EndsWith("/")) baseAddress += "/";

// Add HttpClient - browser-compatible HttpClient for WebAssembly pointing at the API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

// Add existing services
builder.Services.AddScoped<WheelConfigurationService>();

// Add Daily Code Service for access control
builder.Services.AddScoped<DailyCodeService>();

// Add Recipe Service that uses the API
builder.Services.AddScoped<IRecipeService, RecipeService>();

// After building the host we need to restore token into HttpClient so protected endpoints work
var host = builder.Build();
var js = host.Services.GetRequiredService<IJSRuntime>();
var http = host.Services.GetRequiredService<HttpClient>();
var dailyService = host.Services.GetRequiredService<DailyCodeService>();

// Restore token if present
await dailyService.RestoreTokenFromStorageAsync();

await host.RunAsync();
