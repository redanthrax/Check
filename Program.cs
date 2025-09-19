using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;
using CheckWebAssembly;
using CheckWebAssembly.Services;
using CheckWebAssembly.Interop;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using IConfigManager = CheckWebAssembly.Services.IConfigurationManager;
using ConfigManager = CheckWebAssembly.Services.ConfigurationManager;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure services
builder.Services.AddScoped<IChromeExtensionInterop, ChromeExtensionInterop>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IConfigManager, ConfigManager>();

// Configure HTTP client
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Information);
});

// Add root components
var appType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == "App");
if (appType != null) {
    builder.RootComponents.Add(appType, "#app");
}
builder.RootComponents.Add<HeadOutlet>("head::after");

// Build application
var app = builder.Build();

// Initialize extension context after build
try
{
    var jsRuntime = app.Services.GetRequiredService<IJSRuntime>();
    
    // Initialize Check Extension global object
    await jsRuntime.InvokeVoidAsync("eval", @"
        if (!window.CheckExtension) {
            window.CheckExtension = {
                context: null,
                setContext: function(context) {
                    this.context = context;
                    console.log('Check Extension context set to:', context);
                    document.body.setAttribute('data-extension-context', context);
                },
                getContext: function() {
                    return this.context;
                }
            };
        }
    ");
    
    Console.WriteLine("Check WebAssembly Extension initialized successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Extension initialization completed with warnings: {ex.Message}");
}

await app.RunAsync();
