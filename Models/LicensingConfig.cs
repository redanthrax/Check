namespace CheckWebAssembly.Models;

/// <summary>
/// Licensing configuration
/// </summary>
public class LicensingConfig
{
    public string LicenseKey { get; set; } = string.Empty;
    public string LicensedTo { get; set; } = string.Empty;
    public string LicenseType { get; set; } = "enterprise";
    public DateTime? LicenseExpiry { get; set; }
    public int? MaxUsers { get; set; }
    public List<string> Features { get; set; } = new();
}