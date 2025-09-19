using CheckWebAssembly.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CheckWebAssembly.Services;

/// <summary>
/// Configuration manager implementation with caching and enterprise support
/// </summary>
public class ConfigurationManager : IConfigurationManager
{
    private readonly IStorageService _storageService;
    private readonly ILogger<ConfigurationManager> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    
    private ExtensionConfig? _cachedConfig;
    private BrandingConfig? _cachedBrandingConfig;
    private DateTime? _lastConfigLoad;
    private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(5);

    public event EventHandler<ExtensionConfig>? ConfigurationChanged;

    public ConfigurationManager(IStorageService storageService, ILogger<ConfigurationManager> logger)
    {
        _storageService = storageService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        
        // Subscribe to storage changes
        _storageService.StorageChanged += OnStorageChanged;
    }

    public async Task<ExtensionConfig> LoadConfigAsync()
    {
        try
        {
            _logger.LogInformation("Loading extension configuration");
            
            // Try to load from storage first
            var storedConfig = await _storageService.GetAsync<ExtensionConfig>("config");
            
            if (storedConfig != null)
            {
                _logger.LogInformation("Configuration loaded from storage");
                _cachedConfig = storedConfig;
                _lastConfigLoad = DateTime.UtcNow;
                return storedConfig;
            }
            
            // If no stored config, create and save default
            _logger.LogInformation("No stored configuration found, creating default");
            var defaultConfig = GetDefaultConfig();
            await SetConfigAsync(defaultConfig);
            
            return defaultConfig;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration, using default");
            return GetDefaultConfig();
        }
    }

    public async Task<ExtensionConfig> GetConfigAsync()
    {
        // Return cached config if available and not expired
        if (_cachedConfig != null && _lastConfigLoad.HasValue && 
            DateTime.UtcNow - _lastConfigLoad.Value < _cacheTimeout)
        {
            return _cachedConfig;
        }
        
        // Load fresh config
        return await LoadConfigAsync();
    }

    public async Task SetConfigAsync(ExtensionConfig config)
    {
        try
        {
            _logger.LogInformation("Saving extension configuration");
            
            config.LastUpdated = DateTime.UtcNow;
            await _storageService.SetAsync("config", config);
            
            // Update cache
            _cachedConfig = config;
            _lastConfigLoad = DateTime.UtcNow;
            
            _logger.LogInformation("Configuration saved successfully");
            ConfigurationChanged?.Invoke(this, config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save configuration");
            throw;
        }
    }

    public async Task UpdateConfigAsync(ExtensionConfig config)
    {
        try
        {
            // Merge with existing config to preserve settings not in the update
            var currentConfig = await GetConfigAsync();
            
            // Validate enterprise policies before making changes
            var enterpriseConfig = await LoadEnterpriseConfigAsync();
            var hasEnforcedPolicies = enterpriseConfig.ContainsKey("enforcedPolicies");
            
            // Helper to check if a property can be updated
            var canUpdate = async (string propertyName, object newValue) =>
            {
                if (!hasEnforcedPolicies) return true;
                return await ValidateEnterprisePolicy(propertyName, newValue);
            };
            
            // Update properties with enterprise policy validation
            if (config.ExtensionEnabled != currentConfig.ExtensionEnabled &&
                await canUpdate("extensionEnabled", config.ExtensionEnabled))
                currentConfig.ExtensionEnabled = config.ExtensionEnabled;
                
            if (config.EnablePageBlocking != currentConfig.EnablePageBlocking &&
                await canUpdate("enablePageBlocking", config.EnablePageBlocking))
                currentConfig.EnablePageBlocking = config.EnablePageBlocking;
                
            if (config.EnableContentManipulation != currentConfig.EnableContentManipulation &&
                await canUpdate("enableContentManipulation", config.EnableContentManipulation))
                currentConfig.EnableContentManipulation = config.EnableContentManipulation;
                
            if (config.EnableUrlMonitoring != currentConfig.EnableUrlMonitoring &&
                await canUpdate("enableUrlMonitoring", config.EnableUrlMonitoring))
                currentConfig.EnableUrlMonitoring = config.EnableUrlMonitoring;
                
            if (config.ShowNotifications != currentConfig.ShowNotifications &&
                await canUpdate("showNotifications", config.ShowNotifications))
                currentConfig.ShowNotifications = config.ShowNotifications;
                
            if (config.EnableValidPageBadge != currentConfig.EnableValidPageBadge &&
                await canUpdate("enableValidPageBadge", config.EnableValidPageBadge))
                currentConfig.EnableValidPageBadge = config.EnableValidPageBadge;
                
            if (config.EnableDebugLogging != currentConfig.EnableDebugLogging &&
                await canUpdate("enableDebugLogging", config.EnableDebugLogging))
                currentConfig.EnableDebugLogging = config.EnableDebugLogging;
                
            if (config.EnableDeveloperConsoleLogging != currentConfig.EnableDeveloperConsoleLogging &&
                await canUpdate("enableDeveloperConsoleLogging", config.EnableDeveloperConsoleLogging))
                currentConfig.EnableDeveloperConsoleLogging = config.EnableDeveloperConsoleLogging;
                
            if (config.EnableCippReporting != currentConfig.EnableCippReporting &&
                await canUpdate("enableCippReporting", config.EnableCippReporting))
                currentConfig.EnableCippReporting = config.EnableCippReporting;
            
            // Update string properties if they're different and not enterprise-locked
            if (!string.IsNullOrEmpty(config.CippServerUrl) && config.CippServerUrl != currentConfig.CippServerUrl &&
                await canUpdate("cippServerUrl", config.CippServerUrl))
                currentConfig.CippServerUrl = config.CippServerUrl;
                
            if (!string.IsNullOrEmpty(config.CippTenantId) && config.CippTenantId != currentConfig.CippTenantId &&
                await canUpdate("cippTenantId", config.CippTenantId))
                currentConfig.CippTenantId = config.CippTenantId;
                
            if (!string.IsNullOrEmpty(config.CustomRulesUrl) && config.CustomRulesUrl != currentConfig.CustomRulesUrl &&
                await canUpdate("customRulesUrl", config.CustomRulesUrl))
                currentConfig.CustomRulesUrl = config.CustomRulesUrl;
            
            if (config.UpdateInterval != currentConfig.UpdateInterval &&
                await canUpdate("updateInterval", config.UpdateInterval))
                currentConfig.UpdateInterval = config.UpdateInterval;
            
            // Update additional properties from the extended config
            if (config.BlockMaliciousUrls != currentConfig.BlockMaliciousUrls &&
                await canUpdate("blockMaliciousUrls", config.BlockMaliciousUrls))
                currentConfig.BlockMaliciousUrls = config.BlockMaliciousUrls;
                
            if (config.BlockPhishingAttempts != currentConfig.BlockPhishingAttempts &&
                await canUpdate("blockPhishingAttempts", config.BlockPhishingAttempts))
                currentConfig.BlockPhishingAttempts = config.BlockPhishingAttempts;
            
            await SetConfigAsync(currentConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update configuration");
            throw;
        }
    }

    public async Task RefreshConfigAsync()
    {
        try
        {
            _logger.LogInformation("Refreshing configuration from storage");
            
            // Clear cache to force reload
            _cachedConfig = null;
            _lastConfigLoad = null;
            
            await LoadConfigAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh configuration");
            throw;
        }
    }

    public async Task SetDefaultConfigAsync()
    {
        try
        {
            _logger.LogInformation("Setting default configuration");
            
            var defaultConfig = GetDefaultConfig();
            await SetConfigAsync(defaultConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set default configuration");
            throw;
        }
    }

    public async Task<BrandingConfig> GetBrandingConfigAsync()
    {
        try
        {
            // Return cached branding config if available
            if (_cachedBrandingConfig != null)
            {
                return _cachedBrandingConfig;
            }
            
            // Try to load from storage
            var storedBranding = await _storageService.GetAsync<BrandingConfig>("branding");
            
            if (storedBranding != null)
            {
                _cachedBrandingConfig = storedBranding;
                return storedBranding;
            }
            
            // Load from default branding file or create default
            var defaultBranding = GetDefaultBrandingConfig();
            _cachedBrandingConfig = defaultBranding;
            
            return defaultBranding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get branding configuration, using default");
            return GetDefaultBrandingConfig();
        }
    }

    public async Task<BrandingConfig> GetFinalBrandingConfigAsync()
    {
        try
        {
            // Get base branding config
            var brandingConfig = await GetBrandingConfigAsync();
            
            // Check for enterprise managed branding overrides
            var managedConfig = await _storageService.GetManagedAsync();
            
            if (managedConfig.ContainsKey("branding"))
            {
                try
                {
                    if (managedConfig["branding"] is JsonElement brandingElement)
                    {
                        var managedBranding = JsonSerializer.Deserialize<BrandingConfig>(brandingElement, _jsonOptions);
                        if (managedBranding != null)
                        {
                            // Merge managed branding with base config
                            return MergeBrandingConfigs(brandingConfig, managedBranding);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse managed branding configuration");
                }
            }
            
            return brandingConfig;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get final branding configuration");
            return await GetBrandingConfigAsync();
        }
    }

    public async Task<bool> MigrateConfigAsync(string? previousVersion)
    {
        try
        {
            _logger.LogInformation("Migrating configuration from version: {PreviousVersion}", previousVersion ?? "unknown");
            
            // Get current config
            var currentConfig = await GetConfigAsync();
            
            // Perform version-specific migrations
            var migrated = false;
            
            if (string.IsNullOrEmpty(previousVersion) || Version.Parse(previousVersion) < Version.Parse("2.0.0"))
            {
                // Migration from v1.x to v2.0
                _logger.LogInformation("Performing migration from v1.x to v2.0");
                
                // Add any new default settings
                if (currentConfig.UpdateInterval == 0)
                {
                    currentConfig.UpdateInterval = 3600000; // 1 hour default
                    migrated = true;
                }
                
                // Ensure version is updated
                currentConfig.Version = "2.0.0";
                migrated = true;
            }
            
            if (migrated)
            {
                await SetConfigAsync(currentConfig);
                _logger.LogInformation("Configuration migration completed");
            }
            
            return migrated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to migrate configuration");
            return false;
        }
    }

    public async Task<Dictionary<string, object>> LoadEnterpriseConfigAsync()
    {
        try
        {
            _logger.LogInformation("Loading enterprise configuration");
            return await _storageService.GetManagedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load enterprise configuration");
            return new Dictionary<string, object>();
        }
    }

    private ExtensionConfig GetDefaultConfig()
    {
        return new ExtensionConfig
        {
            // Extension settings
            ExtensionEnabled = true,
            DebugMode = false,
            
            // Security settings  
            BlockMaliciousUrls = true,
            BlockPhishingAttempts = true,
            EnableContentManipulation = true,
            EnableUrlMonitoring = true,
            EnablePageBlocking = true,
            
            // Detection settings
            DetectionRules = new ExtensionDetectionSettings
            {
                EnableCustomRules = true,
                CustomRulesUrl = "https://raw.githubusercontent.com/CyberDrain/Check/refs/heads/main/rules/detection-rules.json",
                UpdateInterval = 86400000, // 24 hours
                StrictMode = false
            },
            
            // UI settings
            ShowNotifications = true,
            NotificationDuration = 5000,
            EnableValidPageBadge = true,
            
            // Debug settings
            EnableDebugLogging = false,
            EnableDeveloperConsoleLogging = false,
            
            // Custom rules
            CustomRulesUrl = "",
            UpdateInterval = 3600000, // 1 hour (in milliseconds to match JS)
            
            // Performance settings
            ScanDelay = 100,
            MaxScanDepth = 10,
            
            // Whitelist/Blacklist
            WhitelistedDomains = new List<string>(),
            BlacklistedDomains = new List<string>(),
            
            // Enterprise features
            EnterpriseMode = false,
            CentralManagement = false,
            ReportingEndpoint = "",
            
            // CIPP integration
            EnableCippReporting = false,
            CippServerUrl = null,
            CippTenantId = null,
            
            // Feature flags
            Features = new FeatureFlags
            {
                UrlBlocking = true,
                ContentInjection = true,
                RealTimeScanning = true,
                BehaviorAnalysis = false
            },
            
            // Metadata
            LastUpdated = DateTime.UtcNow,
            Version = "2.0.0"
        };
    }

    private BrandingConfig GetDefaultBrandingConfig()
    {
        return new BrandingConfig
        {
            CompanyName = "CyberDrain",
            ProductName = "Check",
            Version = "2.0.0",
            Description = "Protect against phishing attacks targeting Microsoft 365 login pages with enterprise-grade detection - WebAssembly Edition",
            Branding = new BrandingColors(),
            Assets = new BrandingAssets(),
            Customization = new BrandingCustomization(),
            Features = new BrandingFeatures
            {
                WelcomeMessage = "Welcome to Check, CyberDrain's Microsoft 365 Phishing Protection - Safeguard Your Organization",
                Tagline = "Advanced phishing detection for Microsoft 365 login pages by CyberDrain",
                SecurityBadgeText = "Protected by CyberDrain",
                BlockedPageTitle = "Phishing Attempt Blocked by CyberDrain",
                BlockedPageMessage = "This page appears to be a phishing attempt targeting Microsoft 365 credentials. CyberDrain has blocked access to protect your organization."
            },
            CustomText = new Dictionary<string, object>(),
            SocialMedia = new Dictionary<string, string>(),
            WhiteLabel = new WhiteLabelConfig(),
            Licensing = new LicensingConfig(),
            Deployment = new DeploymentConfig(),
            Analytics = new AnalyticsConfig(),
            Updates = new UpdatesConfig(),
            Metadata = new Dictionary<string, object>
            {
                ["created"] = DateTime.UtcNow.ToString("O"),
                ["modified"] = DateTime.UtcNow.ToString("O"),
                ["author"] = "CheckWebAssembly",
                ["schema_version"] = "2.0"
            }
        };
    }

    private BrandingConfig MergeBrandingConfigs(BrandingConfig baseConfig, BrandingConfig managedConfig)
    {
        // Create a copy of the base config and merge managed settings
        var mergedConfig = JsonSerializer.Deserialize<BrandingConfig>(
            JsonSerializer.Serialize(baseConfig, _jsonOptions), _jsonOptions) ?? baseConfig;

        // Merge core branding properties
        if (!string.IsNullOrEmpty(managedConfig.CompanyName))
            mergedConfig.CompanyName = managedConfig.CompanyName;
        if (!string.IsNullOrEmpty(managedConfig.ProductName))
            mergedConfig.ProductName = managedConfig.ProductName;
        if (!string.IsNullOrEmpty(managedConfig.Description))
            mergedConfig.Description = managedConfig.Description;
        if (!string.IsNullOrEmpty(managedConfig.Version))
            mergedConfig.Version = managedConfig.Version;

        // Merge branding colors if provided
        if (managedConfig.Branding != null)
        {
            if (!string.IsNullOrEmpty(managedConfig.Branding.PrimaryColor))
                mergedConfig.Branding.PrimaryColor = managedConfig.Branding.PrimaryColor;
            if (!string.IsNullOrEmpty(managedConfig.Branding.SecondaryColor))
                mergedConfig.Branding.SecondaryColor = managedConfig.Branding.SecondaryColor;
        }

        // Merge assets if provided
        if (managedConfig.Assets != null)
        {
            if (!string.IsNullOrEmpty(managedConfig.Assets.LogoUrl))
                mergedConfig.Assets.LogoUrl = managedConfig.Assets.LogoUrl;
            if (!string.IsNullOrEmpty(managedConfig.Assets.IconUrl))
                mergedConfig.Assets.IconUrl = managedConfig.Assets.IconUrl;
        }

        // Merge features if provided
        if (managedConfig.Features != null)
        {
            if (!string.IsNullOrEmpty(managedConfig.Features.WelcomeMessage))
                mergedConfig.Features.WelcomeMessage = managedConfig.Features.WelcomeMessage;
            if (!string.IsNullOrEmpty(managedConfig.Features.BlockedPageTitle))
                mergedConfig.Features.BlockedPageTitle = managedConfig.Features.BlockedPageTitle;
            if (!string.IsNullOrEmpty(managedConfig.Features.BlockedPageMessage))
                mergedConfig.Features.BlockedPageMessage = managedConfig.Features.BlockedPageMessage;
        }

        // Merge custom text if provided
        if (managedConfig.CustomText != null && managedConfig.CustomText.Count > 0)
        {
            foreach (var kvp in managedConfig.CustomText)
            {
                mergedConfig.CustomText[kvp.Key] = kvp.Value;
            }
        }

        return mergedConfig;
    }

    private void OnStorageChanged(object? sender, StorageChangedEventArgs e)
    {
        if (e.Key == "config" && e.Area == StorageArea.Local)
        {
            // Clear cache when config changes externally
            _cachedConfig = null;
            _lastConfigLoad = null;
            
            // Notify about configuration change if we can parse the new value
            if (e.NewValue is ExtensionConfig newConfig)
            {
                ConfigurationChanged?.Invoke(this, newConfig);
            }
        }
        else if (e.Key == "branding" && e.Area == StorageArea.Local)
        {
            // Clear branding cache
            _cachedBrandingConfig = null;
        }
    }

    // Additional methods for feature parity with JavaScript version
    
    public async Task<string> ExportConfigurationAsync()
    {
        try
        {
            var config = await GetConfigAsync();
            var branding = await GetBrandingConfigAsync();
            
            var exportData = new
            {
                config = config,
                branding = branding,
                timestamp = DateTime.UtcNow.ToString("O"),
                version = "2.0.0"
            };
            
            return JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export configuration");
            throw;
        }
    }
    
    public async Task<bool> ImportConfigurationAsync(string configJson)
    {
        try
        {
            _logger.LogInformation("Importing configuration from JSON");
            
            var importData = JsonSerializer.Deserialize<JsonElement>(configJson, _jsonOptions);
            
            // Validate import data structure
            if (!importData.TryGetProperty("config", out var configElement))
            {
                throw new ArgumentException("Invalid configuration format - missing config property");
            }
            
            // Deserialize and update configuration
            var config = JsonSerializer.Deserialize<ExtensionConfig>(configElement, _jsonOptions);
            if (config != null)
            {
                await SetConfigAsync(config);
            }
            
            // Import branding if provided
            if (importData.TryGetProperty("branding", out var brandingElement))
            {
                var branding = JsonSerializer.Deserialize<BrandingConfig>(brandingElement, _jsonOptions);
                if (branding != null)
                {
                    await _storageService.SetAsync("branding", branding);
                    _cachedBrandingConfig = branding;
                }
            }
            
            _logger.LogInformation("Configuration imported successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import configuration");
            throw;
        }
    }
    
    public async Task<bool> IsDevelopmentModeAsync()
    {
        try
        {
            // Check if we're in development mode based on environment or configuration
            // This could be determined by build configuration, environment variables, etc.
            var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            return isDev;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> GetSimulateEnterpriseModeAsync()
    {
        try
        {
            var isDev = await IsDevelopmentModeAsync();
            if (!isDev) return false;
            
            var simulateMode = await _storageService.GetAsync<bool?>("simulateEnterpriseMode");
            return simulateMode ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get simulate enterprise mode setting");
            return false;
        }
    }
    
    public async Task SetSimulateEnterpriseModeAsync(bool enabled)
    {
        try
        {
            var isDev = await IsDevelopmentModeAsync();
            if (!isDev)
            {
                _logger.LogWarning("Simulate enterprise mode is only available in development");
                return;
            }
            
            await _storageService.SetAsync("simulateEnterpriseMode", enabled);
            _logger.LogInformation("Simulate enterprise mode set to: {Enabled}", enabled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set simulate enterprise mode");
            throw;
        }
    }
    
    public bool IsVersionLessThan(string version1, string version2)
    {
        try
        {
            var v1 = Version.Parse(version1);
            var v2 = Version.Parse(version2);
            return v1 < v2;
        }
        catch
        {
            // If version parsing fails, assume older version
            return true;
        }
    }
    
    public async Task<bool> ValidateEnterprisePolicy(string policyName, object newValue)
    {
        try
        {
            var enterpriseConfig = await LoadEnterpriseConfigAsync();
            
            if (enterpriseConfig.ContainsKey("enforcedPolicies") &&
                enterpriseConfig["enforcedPolicies"] is JsonElement policiesElement)
            {
                if (policiesElement.TryGetProperty(policyName, out var policyElement) &&
                    policyElement.TryGetProperty("locked", out var lockedElement) &&
                    lockedElement.GetBoolean())
                {
                    _logger.LogWarning("Policy '{PolicyName}' is locked by enterprise configuration", policyName);
                    return false;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate enterprise policy for {PolicyName}", policyName);
            return true; // Allow changes if validation fails
        }
    }
}
