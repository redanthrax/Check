namespace CheckWebAssembly.Models;

/// <summary>
/// Branding configuration model
/// </summary>
public class BrandingConfig
{
    public string CompanyName { get; set; } = "CyberDrain";
    public string ProductName { get; set; } = "Check";
    public string Version { get; set; } = "2.0.0";
    public string Description { get; set; } = string.Empty;
    public BrandingColors Branding { get; set; } = new();
    public BrandingAssets Assets { get; set; } = new();
    public BrandingCustomization Customization { get; set; } = new();
    public BrandingFeatures Features { get; set; } = new();
    public Dictionary<string, object> CustomText { get; set; } = new();
    public Dictionary<string, string> SocialMedia { get; set; } = new();
    public WhiteLabelConfig WhiteLabel { get; set; } = new();
    public LicensingConfig Licensing { get; set; } = new();
    public DeploymentConfig Deployment { get; set; } = new();
    public AnalyticsConfig Analytics { get; set; } = new();
    public UpdatesConfig Updates { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}