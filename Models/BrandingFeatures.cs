namespace CheckWebAssembly.Models;

/// <summary>
/// Branding features configuration
/// </summary>
public class BrandingFeatures
{
    public string WelcomeMessage { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string SecurityBadgeText { get; set; } = string.Empty;
    public string BlockedPageTitle { get; set; } = string.Empty;
    public string BlockedPageMessage { get; set; } = string.Empty;
}