namespace CheckWebAssembly.Models;

/// <summary>
/// Extension configuration model
/// </summary>
public class ExtensionConfig
{
    // Extension settings
    public bool ExtensionEnabled { get; set; } = true;
    public bool DebugMode { get; set; } = false;
    
    // Security settings
    public bool BlockMaliciousUrls { get; set; } = true;
    public bool BlockPhishingAttempts { get; set; } = true;
    public bool EnablePageBlocking { get; set; } = true;
    public bool EnableContentManipulation { get; set; } = true;
    public bool EnableUrlMonitoring { get; set; } = true;
    
    // Detection settings
    public ExtensionDetectionSettings DetectionRules { get; set; } = new();
    
    // UI settings
    public bool ShowNotifications { get; set; } = true;
    public int NotificationDuration { get; set; } = 5000;
    public bool EnableValidPageBadge { get; set; } = false;
    
    // Debug settings
    public bool EnableDebugLogging { get; set; } = false;
    public bool EnableDeveloperConsoleLogging { get; set; } = false;
    
    // Custom rules
    public string? CustomRulesUrl { get; set; }
    public int UpdateInterval { get; set; } = 3600000; // 1 hour
    
    // Performance settings
    public int ScanDelay { get; set; } = 100;
    public int MaxScanDepth { get; set; } = 10;
    
    // Whitelist/Blacklist
    public List<string> WhitelistedDomains { get; set; } = new();
    public List<string> BlacklistedDomains { get; set; } = new();
    
    // Enterprise features
    public bool EnterpriseMode { get; set; } = false;
    public bool CentralManagement { get; set; } = false;
    public string ReportingEndpoint { get; set; } = "";
    
    // CIPP integration
    public bool EnableCippReporting { get; set; } = false;
    public string? CippServerUrl { get; set; }
    public string? CippTenantId { get; set; }
    
    // Feature flags
    public FeatureFlags Features { get; set; } = new();
    
    // Metadata
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = "2.0.0";
}

/// <summary>
/// Extension detection settings configuration
/// </summary>
public class ExtensionDetectionSettings
{
    public bool EnableCustomRules { get; set; } = true;
    public string CustomRulesUrl { get; set; } = "";
    public int UpdateInterval { get; set; } = 86400000; // 24 hours
    public bool StrictMode { get; set; } = false;
}

/// <summary>
/// Feature flags configuration
/// </summary>
public class FeatureFlags
{
    public bool UrlBlocking { get; set; } = true;
    public bool ContentInjection { get; set; } = true;
    public bool RealTimeScanning { get; set; } = true;
    public bool BehaviorAnalysis { get; set; } = false;
}
