using CheckWebAssembly.Interop;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CheckWebAssembly.Services;

/// <summary>
/// Chrome extension storage service implementation
/// </summary>
public class StorageService : IStorageService
{
    private readonly IChromeExtensionInterop _chromeInterop;
    private readonly ILogger<StorageService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public event EventHandler<StorageChangedEventArgs>? StorageChanged;

    public StorageService(IChromeExtensionInterop chromeInterop, ILogger<StorageService> logger)
    {
        _chromeInterop = chromeInterop;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    // Local storage operations
    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            return await _chromeInterop.GetStorageLocalAsync<T>(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get storage value for key: {Key}", key);
            return default;
        }
    }

    public async Task<Dictionary<string, T>> GetMultipleAsync<T>(params string[] keys)
    {
        try
        {
            var result = await _chromeInterop.GetStorageLocalAsync(keys);
            var typedResult = new Dictionary<string, T>();

            foreach (var kvp in result)
            {
                if (kvp.Value is JsonElement jsonElement)
                {
                    try
                    {
                        var value = JsonSerializer.Deserialize<T>(jsonElement, _jsonOptions);
                        if (value != null)
                        {
                            typedResult[kvp.Key] = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize value for key: {Key}", kvp.Key);
                    }
                }
                else if (kvp.Value is T directValue)
                {
                    typedResult[kvp.Key] = directValue;
                }
            }

            return typedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get multiple storage values for keys: {Keys}", string.Join(", ", keys));
            return new Dictionary<string, T>();
        }
    }

    public async Task SetAsync<T>(string key, T value)
    {
        try
        {
            var oldValue = await GetAsync<T>(key);
            await _chromeInterop.SetStorageLocalAsync(key, value);
            
            // Notify about the change
            StorageChanged?.Invoke(this, new StorageChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = value,
                Area = StorageArea.Local
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set storage value for key: {Key}", key);
        }
    }

    public async Task SetMultipleAsync(Dictionary<string, object> items)
    {
        try
        {
            // Get old values for change notifications
            var oldValues = new Dictionary<string, object?>();
            foreach (var key in items.Keys)
            {
                oldValues[key] = await GetAsync<object>(key);
            }

            await _chromeInterop.SetStorageLocalAsync(items);

            // Notify about changes
            foreach (var kvp in items)
            {
                StorageChanged?.Invoke(this, new StorageChangedEventArgs
                {
                    Key = kvp.Key,
                    OldValue = oldValues.GetValueOrDefault(kvp.Key),
                    NewValue = kvp.Value,
                    Area = StorageArea.Local
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set multiple storage values");
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            var oldValue = await GetAsync<object>(key);
            await _chromeInterop.RemoveStorageLocalAsync(key);

            // Notify about the change
            StorageChanged?.Invoke(this, new StorageChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = null,
                Area = StorageArea.Local
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove storage value for key: {Key}", key);
        }
    }

    public async Task RemoveMultipleAsync(params string[] keys)
    {
        try
        {
            // Get old values for change notifications
            var oldValues = new Dictionary<string, object?>();
            foreach (var key in keys)
            {
                oldValues[key] = await GetAsync<object>(key);
            }

            await _chromeInterop.RemoveStorageLocalAsync(keys);

            // Notify about changes
            foreach (var key in keys)
            {
                StorageChanged?.Invoke(this, new StorageChangedEventArgs
                {
                    Key = key,
                    OldValue = oldValues.GetValueOrDefault(key),
                    NewValue = null,
                    Area = StorageArea.Local
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove multiple storage values for keys: {Keys}", string.Join(", ", keys));
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await _chromeInterop.ClearStorageLocalAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear storage");
        }
    }

    // Session storage operations
    public async Task<T?> GetSessionAsync<T>(string key)
    {
        try
        {
            return await _chromeInterop.GetStorageSessionAsync<T>(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get session storage value for key: {Key}", key);
            return default;
        }
    }

    public async Task SetSessionAsync<T>(string key, T value)
    {
        try
        {
            var oldValue = await GetSessionAsync<T>(key);
            await _chromeInterop.SetStorageSessionAsync(key, value);

            // Notify about the change
            StorageChanged?.Invoke(this, new StorageChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = value,
                Area = StorageArea.Session
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set session storage value for key: {Key}", key);
        }
    }

    public async Task RemoveSessionAsync(string key)
    {
        try
        {
            var oldValue = await GetSessionAsync<object>(key);
            await _chromeInterop.RemoveStorageSessionAsync(key);

            // Notify about the change
            StorageChanged?.Invoke(this, new StorageChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = null,
                Area = StorageArea.Session
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove session storage value for key: {Key}", key);
        }
    }

    // Managed storage operations (enterprise policies)
    public async Task<Dictionary<string, object>> GetManagedAsync()
    {
        try
        {
            return await _chromeInterop.GetStorageManagedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get managed storage");
            return new Dictionary<string, object>();
        }
    }
}