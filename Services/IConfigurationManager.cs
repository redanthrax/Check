using CheckWebAssembly.Models;

namespace CheckWebAssembly.Services;

/// <summary>
/// Interface for configuration management service
/// </summary>
public interface IConfigurationManager
{
    /// <summary>
    /// Loads configuration from storage, creating default if none exists
    /// </summary>
    Task<ExtensionConfig> LoadConfigAsync();
    
    /// <summary>
    /// Gets cached configuration or loads it if not cached
    /// </summary>
    Task<ExtensionConfig> GetConfigAsync();
    
    /// <summary>
    /// Saves configuration to storage and updates cache
    /// </summary>
    Task SetConfigAsync(ExtensionConfig config);
    
    /// <summary>
    /// Updates existing configuration with partial changes
    /// </summary>
    Task UpdateConfigAsync(ExtensionConfig config);
    
    /// <summary>
    /// Forces reload of configuration from storage
    /// </summary>
    Task RefreshConfigAsync();
    
    /// <summary>
    /// Resets configuration to default values
    /// </summary>
    Task SetDefaultConfigAsync();
    
    /// <summary>
    /// Gets branding configuration
    /// </summary>
    Task<BrandingConfig> GetBrandingConfigAsync();
    
    /// <summary>
    /// Gets final branding configuration with enterprise overrides applied
    /// </summary>
    Task<BrandingConfig> GetFinalBrandingConfigAsync();
    
    /// <summary>
    /// Migrates configuration from a previous version
    /// </summary>
    Task<bool> MigrateConfigAsync(string? previousVersion);
    
    /// <summary>
    /// Loads enterprise managed configuration from policy/registry
    /// </summary>
    Task<Dictionary<string, object>> LoadEnterpriseConfigAsync();
    
    /// <summary>
    /// Exports the current configuration as JSON string
    /// </summary>
    Task<string> ExportConfigurationAsync();
    
    /// <summary>
    /// Imports configuration from JSON string
    /// </summary>
    Task<bool> ImportConfigurationAsync(string configJson);
    
    /// <summary>
    /// Checks if the application is running in development mode
    /// </summary>
    Task<bool> IsDevelopmentModeAsync();
    
    /// <summary>
    /// Gets the simulate enterprise mode setting (development only)
    /// </summary>
    Task<bool> GetSimulateEnterpriseModeAsync();
    
    /// <summary>
    /// Sets the simulate enterprise mode setting (development only)
    /// </summary>
    Task SetSimulateEnterpriseModeAsync(bool enabled);
    
    /// <summary>
    /// Compares two version strings
    /// </summary>
    bool IsVersionLessThan(string version1, string version2);
    
    /// <summary>
    /// Validates if an enterprise policy allows changing a setting
    /// </summary>
    Task<bool> ValidateEnterprisePolicy(string policyName, object newValue);
    
    /// <summary>
    /// Event fired when configuration changes
    /// </summary>
    event EventHandler<ExtensionConfig>? ConfigurationChanged;
}
