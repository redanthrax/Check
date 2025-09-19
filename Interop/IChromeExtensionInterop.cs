using System.Text.Json;

namespace CheckWebAssembly.Interop;

/// <summary>
/// Interface for Chrome extension API interop operations
/// </summary>
public interface IChromeExtensionInterop
{
    // Storage APIs
    Task<T?> GetStorageLocalAsync<T>(string key);
    Task<Dictionary<string, object>> GetStorageLocalAsync(string[] keys);
    Task SetStorageLocalAsync<T>(string key, T value);
    Task SetStorageLocalAsync(Dictionary<string, object> items);
    Task RemoveStorageLocalAsync(string key);
    Task RemoveStorageLocalAsync(string[] keys);
    Task ClearStorageLocalAsync();
    
    Task<T?> GetStorageSessionAsync<T>(string key);
    Task SetStorageSessionAsync<T>(string key, T value);
    Task RemoveStorageSessionAsync(string key);
    
    Task<Dictionary<string, object>> GetStorageManagedAsync();
    
    // Runtime APIs
    Task<T?> SendMessageAsync<T>(object message);
    Task<T?> SendMessageToTabAsync<T>(int tabId, object message);
    ValueTask<string> GetExtensionIdAsync();
    ValueTask<string> GetUrlAsync(string path);
    
    // Tabs APIs
    Task<ChromeTab?> GetActiveTabAsync();
    Task<ChromeTab[]> QueryTabsAsync(object query);
    Task<ChromeTab?> GetTabAsync(int tabId);
    Task UpdateTabAsync(int tabId, object updateProperties);
    Task CreateTabAsync(object createProperties);
    
    // Action APIs
    Task SetBadgeTextAsync(int tabId, string text);
    Task SetBadgeBackgroundColorAsync(int tabId, string color);
    Task SetActionTitleAsync(string title);
    Task SetActionIconAsync(object iconData);
    
    // Scripting APIs
    Task ExecuteScriptAsync(int tabId, string[] files);
    Task InsertCssAsync(int tabId, string[] files);
    Task RemoveCssAsync(int tabId, string[] files);
    
    // Alarms APIs
    Task CreateAlarmAsync(string name, object alarmInfo);
    Task ClearAlarmAsync(string name);
    Task<ChromeAlarm[]> GetAllAlarmsAsync();
    
    // Identity APIs
    Task<ChromeUserInfo?> GetProfileUserInfoAsync();
    
    // WebRequest APIs - Note: These might need special handling in WebAssembly
    void AddWebRequestListener(string eventType, string[] urls, string[] extraInfoSpec);
    void RemoveWebRequestListener(string eventType);
    
    // DOM and Page APIs (for content scripts)
    Task<string> GetPageSourceAsync();
    Task<HtmlElement[]> QuerySelectorsAsync(string selector);
    Task<HtmlElement?> QuerySelectorAsync(string selector);
    Task SetElementTextAsync(string selector, string text);
    Task SetElementHtmlAsync(string selector, string html);
    Task AddEventListenerAsync(string selector, string eventType, string callbackId);
    Task RemoveEventListenerAsync(string selector, string eventType, string callbackId);
    
    // Fetch API with timeout support
    Task<HttpResponseData> FetchAsync(string url, FetchOptions? options = null);
}

public record ChromeTab(int Id, string Url, string Title, bool Active, int WindowId);
public record ChromeAlarm(string Name, double ScheduledTime, double? PeriodInMinutes);
public record ChromeUserInfo(string? Email, string? Id);
public record HtmlElement(string TagName, string? Id, string? ClassName, string InnerText, string InnerHtml);
public record HttpResponseData(int Status, string StatusText, Dictionary<string, string> Headers, string Body, bool Ok);

public class FetchOptions {
    public string Method { get; set; } = "GET";
    public Dictionary<string, string>? Headers { get; set; }
    public string? Body { get; set; }
    public int TimeoutMs { get; set; } = 5000;
    public string? Signal { get; set; }
}