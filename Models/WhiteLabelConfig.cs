namespace CheckWebAssembly.Models;

/// <summary>
/// White label configuration
/// </summary>
public class WhiteLabelConfig
{
    public bool Enabled { get; set; } = true;
    public bool AllowCustomColors { get; set; } = true;
    public bool AllowCustomLogos { get; set; } = true;
    public bool AllowCustomText { get; set; } = true;
    public bool AllowCustomIcons { get; set; } = true;
    public bool AllowCustomCss { get; set; } = true;
    public bool PreserveAttribution { get; set; } = false;
    public List<string> CustomizableElements { get; set; } = new();
}