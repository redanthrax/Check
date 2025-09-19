namespace CheckWebAssembly.Models;

/// <summary>
/// Deployment configuration
/// </summary>
public class DeploymentConfig
{
    public List<string> SupportedPlatforms { get; set; } = new();
    public string MinimumVersion { get; set; } = "88";
    public int ManifestVersion { get; set; } = 3;
    public List<string> DeploymentMethods { get; set; } = new();
}