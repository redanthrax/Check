namespace CheckWebAssembly.Services;

/// <summary>
/// Interface for Chrome extension storage operations
/// </summary>
public interface IStorageService
{
    // Local storage operations
    Task<T?> GetAsync<T>(string key);
    Task<Dictionary<string, T>> GetMultipleAsync<T>(params string[] keys);
    Task SetAsync<T>(string key, T value);
    Task SetMultipleAsync(Dictionary<string, object> items);
    Task RemoveAsync(string key);
    Task RemoveMultipleAsync(params string[] keys);
    Task ClearAsync();

    // Session storage operations
    Task<T?> GetSessionAsync<T>(string key);
    Task SetSessionAsync<T>(string key, T value);
    Task RemoveSessionAsync(string key);

    // Managed storage operations (enterprise policies)
    Task<Dictionary<string, object>> GetManagedAsync();

    // Event handling for storage changes
    event EventHandler<StorageChangedEventArgs>? StorageChanged;
}

/// <summary>
/// Event arguments for storage change notifications
/// </summary>
public class StorageChangedEventArgs : EventArgs
{
    public required string Key { get; init; }
    public object? OldValue { get; init; }
    public object? NewValue { get; init; }
    public StorageArea Area { get; init; }
}

/// <summary>
/// Chrome storage areas
/// </summary>
public enum StorageArea
{
    Local,
    Session,
    Managed
}