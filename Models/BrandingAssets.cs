namespace CheckWebAssembly.Models;

/// <summary>
/// Branding assets configuration
/// </summary>
public class BrandingAssets
{
    public string LogoUrl { get; set; } = "images/icon48.png";
    public string IconUrl { get; set; } = "images/icon32.png";
    public string FaviconUrl { get; set; } = "images/icon16.png";
    public string BannerUrl { get; set; } = "images/banner.png";
    public List<string> ScreenshotUrls { get; set; } = new();
}