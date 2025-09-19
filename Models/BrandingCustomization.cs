namespace CheckWebAssembly.Models;

/// <summary>
/// Branding customization settings
/// </summary>
public class BrandingCustomization
{
    public bool ShowCompanyBranding { get; set; } = true;
    public bool AllowUserCustomization { get; set; } = true;
    public bool EnableWhiteLabeling { get; set; } = true;
    public bool CustomCssEnabled { get; set; } = true;
    public bool CustomIconsEnabled { get; set; } = true;
}