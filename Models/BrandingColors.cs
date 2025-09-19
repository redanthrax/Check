namespace CheckWebAssembly.Models;

/// <summary>
/// Branding colors configuration
/// </summary>
public class BrandingColors
{
    public string PrimaryColor { get; set; } = "#F77F00";
    public string PrimaryHover { get; set; } = "#E56F00";
    public string PrimaryLight { get; set; } = "rgba(247, 127, 0, 0.1)";
    public string PrimaryDark { get; set; } = "#D96800";
    public string SecondaryColor { get; set; } = "#003049";
    public string SecondaryHover { get; set; } = "#004B73";
    public string SecondaryLight { get; set; } = "rgba(0, 48, 73, 0.1)";
    public string SecondaryDark { get; set; } = "#002236";
    public string AccentColor { get; set; } = "#005C63";
    public string SuccessColor { get; set; } = "#005C63";
    public string WarningColor { get; set; } = "#F77F00";
    public string ErrorColor { get; set; } = "#DC2626";
    public string TextPrimary { get; set; } = "#FFFFFF";
    public string TextSecondary { get; set; } = "#9CA3AF";
    public string TextMuted { get; set; } = "#6B7280";
    public string TextInverse { get; set; } = "#003049";
    public string BgPrimary { get; set; } = "#003049";
    public string BgSecondary { get; set; } = "rgba(255, 255, 255, 0.05)";
    public string BgSurface { get; set; } = "rgba(255, 255, 255, 0.03)";
    public string Border { get; set; } = "rgba(255, 255, 255, 0.1)";
    public string BorderHover { get; set; } = "rgba(247, 127, 0, 0.3)";
}