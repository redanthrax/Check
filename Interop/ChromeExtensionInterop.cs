using Microsoft.JSInterop;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace CheckWebAssembly.Interop;

/// <summary>
/// Implementation of Chrome extension API interop using JavaScript
/// </summary>
public class ChromeExtensionInterop : IChromeExtensionInterop, IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<ChromeExtensionInterop> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public ChromeExtensionInterop(IJSRuntime jsRuntime, ILogger<ChromeExtensionInterop> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    // Storage APIs
    public async Task<T?> GetStorageLocalAsync<T>(string key)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.getStorageLocal", key);
            return string.IsNullOrEmpty(result) ? default : JsonSerializer.Deserialize<T>(result, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get storage local item: {Key}", key);
            return default;
        }
    }

    public async Task<Dictionary<string, object>> GetStorageLocalAsync(string[] keys)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.getStorageLocalMultiple", (object)keys);
            return string.IsNullOrEmpty(result) 
                ? new Dictionary<string, object>() 
                : JsonSerializer.Deserialize<Dictionary<string, object>>(result, _jsonOptions) ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get storage local items: {Keys}", string.Join(", ", keys));
            return new Dictionary<string, object>();
        }
    }

    public async Task SetStorageLocalAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setStorageLocal", key, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set storage local item: {Key}", key);
        }
    }

    public async Task SetStorageLocalAsync(Dictionary<string, object> items)
    {
        try
        {
            var json = JsonSerializer.Serialize(items, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setStorageLocalMultiple", json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set storage local items");
        }
    }

    public async Task RemoveStorageLocalAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.removeStorageLocal", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove storage local item: {Key}", key);
        }
    }

    public async Task RemoveStorageLocalAsync(string[] keys)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.removeStorageLocalMultiple", (object)keys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove storage local items: {Keys}", string.Join(", ", keys));
        }
    }

    public async Task ClearStorageLocalAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.clearStorageLocal");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear storage local");
        }
    }

    // Session storage
    public async Task<T?> GetStorageSessionAsync<T>(string key)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.getStorageSession", key);
            return string.IsNullOrEmpty(result) ? default : JsonSerializer.Deserialize<T>(result, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get storage session item: {Key}", key);
            return default;
        }
    }

    public async Task SetStorageSessionAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setStorageSession", key, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set storage session item: {Key}", key);
        }
    }

    public async Task RemoveStorageSessionAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.removeStorageSession", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove storage session item: {Key}", key);
        }
    }

    // Managed storage
    public async Task<Dictionary<string, object>> GetStorageManagedAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.getStorageManaged");
            return string.IsNullOrEmpty(result) 
                ? new Dictionary<string, object>() 
                : JsonSerializer.Deserialize<Dictionary<string, object>>(result, _jsonOptions) ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get managed storage");
            return new Dictionary<string, object>();
        }
    }

    // Runtime APIs
    public async Task<T?> SendMessageAsync<T>(object message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message, _jsonOptions);
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.sendMessage", messageJson);
            return string.IsNullOrEmpty(result) ? default : JsonSerializer.Deserialize<T>(result, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message");
            return default;
        }
    }

    public async Task<T?> SendMessageToTabAsync<T>(int tabId, object message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message, _jsonOptions);
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.sendMessageToTab", tabId, messageJson);
            return string.IsNullOrEmpty(result) ? default : JsonSerializer.Deserialize<T>(result, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to tab: {TabId}", tabId);
            return default;
        }
    }

    public async ValueTask<string> GetExtensionIdAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("chromeInterop.getExtensionId");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get extension ID");
            return string.Empty;
        }
    }

    public async ValueTask<string> GetUrlAsync(string path)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("chromeInterop.getUrl", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get URL for path: {Path}", path);
            return string.Empty;
        }
    }

    // Tabs APIs
    public async Task<ChromeTab?> GetActiveTabAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.getActiveTab");
            return string.IsNullOrEmpty(result) ? null : JsonSerializer.Deserialize<ChromeTab>(result, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get active tab");
            return null;
        }
    }

    public async Task<ChromeTab[]> QueryTabsAsync(object query)
    {
        try
        {
            var queryJson = JsonSerializer.Serialize(query, _jsonOptions);
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.queryTabs", queryJson);
            return string.IsNullOrEmpty(result) 
                ? Array.Empty<ChromeTab>() 
                : JsonSerializer.Deserialize<ChromeTab[]>(result, _jsonOptions) ?? Array.Empty<ChromeTab>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query tabs");
            return Array.Empty<ChromeTab>();
        }
    }

    public async Task<ChromeTab?> GetTabAsync(int tabId)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.getTab", tabId);
            return string.IsNullOrEmpty(result) ? null : JsonSerializer.Deserialize<ChromeTab>(result, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get tab: {TabId}", tabId);
            return null;
        }
    }

    public async Task UpdateTabAsync(int tabId, object updateProperties)
    {
        try
        {
            var propertiesJson = JsonSerializer.Serialize(updateProperties, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("chromeInterop.updateTab", tabId, propertiesJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update tab: {TabId}", tabId);
        }
    }

    public async Task CreateTabAsync(object createProperties)
    {
        try
        {
            var propertiesJson = JsonSerializer.Serialize(createProperties, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("chromeInterop.createTab", propertiesJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create tab");
        }
    }

    // Action APIs
    public async Task SetBadgeTextAsync(int tabId, string text)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setBadgeText", tabId, text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set badge text for tab: {TabId}", tabId);
        }
    }

    public async Task SetBadgeBackgroundColorAsync(int tabId, string color)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setBadgeBackgroundColor", tabId, color);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set badge background color for tab: {TabId}", tabId);
        }
    }

    public async Task SetActionTitleAsync(string title)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setActionTitle", title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set action title");
        }
    }

    public async Task SetActionIconAsync(object iconData)
    {
        try
        {
            var iconJson = JsonSerializer.Serialize(iconData, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setActionIcon", iconJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set action icon");
        }
    }

    // Scripting APIs
    public async Task ExecuteScriptAsync(int tabId, string[] files)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.executeScript", tabId, (object)files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute script in tab: {TabId}", tabId);
        }
    }

    public async Task InsertCssAsync(int tabId, string[] files)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.insertCss", tabId, (object)files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert CSS in tab: {TabId}", tabId);
        }
    }

    public async Task RemoveCssAsync(int tabId, string[] files)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.removeCss", tabId, (object)files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove CSS from tab: {TabId}", tabId);
        }
    }

    // Alarms APIs
    public async Task CreateAlarmAsync(string name, object alarmInfo)
    {
        try
        {
            var alarmJson = JsonSerializer.Serialize(alarmInfo, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("chromeInterop.createAlarm", name, alarmJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create alarm: {Name}", name);
        }
    }

    public async Task ClearAlarmAsync(string name)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.clearAlarm", name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear alarm: {Name}", name);
        }
    }

    public async Task<ChromeAlarm[]> GetAllAlarmsAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.getAllAlarms");
            return string.IsNullOrEmpty(result) 
                ? Array.Empty<ChromeAlarm>() 
                : JsonSerializer.Deserialize<ChromeAlarm[]>(result, _jsonOptions) ?? Array.Empty<ChromeAlarm>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all alarms");
            return Array.Empty<ChromeAlarm>();
        }
    }

    // Identity APIs
    public async Task<ChromeUserInfo?> GetProfileUserInfoAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.getProfileUserInfo");
            return string.IsNullOrEmpty(result) ? null : JsonSerializer.Deserialize<ChromeUserInfo>(result, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get profile user info");
            return null;
        }
    }

    // WebRequest APIs
    public void AddWebRequestListener(string eventType, string[] urls, string[] extraInfoSpec)
    {
        try
        {
            _jsRuntime.InvokeVoidAsync("chromeInterop.addWebRequestListener", eventType, (object)urls, (object)extraInfoSpec);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add web request listener: {EventType}", eventType);
        }
    }

    public void RemoveWebRequestListener(string eventType)
    {
        try
        {
            _jsRuntime.InvokeVoidAsync("chromeInterop.removeWebRequestListener", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove web request listener: {EventType}", eventType);
        }
    }

    // DOM and Page APIs
    public async Task<string> GetPageSourceAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("chromeInterop.getPageSource");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get page source");
            return string.Empty;
        }
    }

    public async Task<HtmlElement[]> QuerySelectorsAsync(string selector)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.querySelectors", selector);
            return string.IsNullOrEmpty(result) 
                ? Array.Empty<HtmlElement>() 
                : JsonSerializer.Deserialize<HtmlElement[]>(result, _jsonOptions) ?? Array.Empty<HtmlElement>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query selectors: {Selector}", selector);
            return Array.Empty<HtmlElement>();
        }
    }

    public async Task<HtmlElement?> QuerySelectorAsync(string selector)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.querySelector", selector);
            return string.IsNullOrEmpty(result) ? null : JsonSerializer.Deserialize<HtmlElement>(result, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query selector: {Selector}", selector);
            return null;
        }
    }

    public async Task SetElementTextAsync(string selector, string text)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setElementText", selector, text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set element text: {Selector}", selector);
        }
    }

    public async Task SetElementHtmlAsync(string selector, string html)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.setElementHtml", selector, html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set element HTML: {Selector}", selector);
        }
    }

    public async Task AddEventListenerAsync(string selector, string eventType, string callbackId)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.addEventListener", selector, eventType, callbackId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add event listener: {Selector} {EventType}", selector, eventType);
        }
    }

    public async Task RemoveEventListenerAsync(string selector, string eventType, string callbackId)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.removeEventListener", selector, eventType, callbackId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove event listener: {Selector} {EventType}", selector, eventType);
        }
    }

    // Fetch API
    public async Task<HttpResponseData> FetchAsync(string url, FetchOptions? options = null)
    {
        try
        {
            options ??= new FetchOptions();
            var optionsJson = JsonSerializer.Serialize(options, _jsonOptions);
            var result = await _jsRuntime.InvokeAsync<string>("chromeInterop.fetch", url, optionsJson);
            
            if (string.IsNullOrEmpty(result))
            {
                return new HttpResponseData(0, "No response", new Dictionary<string, string>(), string.Empty, false);
            }
            
            return JsonSerializer.Deserialize<HttpResponseData>(result, _jsonOptions) 
                   ?? new HttpResponseData(0, "Failed to parse response", new Dictionary<string, string>(), string.Empty, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch: {Url}", url);
            return new HttpResponseData(0, ex.Message, new Dictionary<string, string>(), string.Empty, false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("chromeInterop.dispose");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispose Chrome interop");
        }
    }
}