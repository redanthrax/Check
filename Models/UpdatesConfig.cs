namespace CheckWebAssembly.Models;

/// <summary>
/// Updates configuration
/// </summary>
public class UpdatesConfig
{
    public bool AutoUpdateEnabled { get; set; } = true;
    public string UpdateChannel { get; set; } = "stable";
    public long UpdateCheckInterval { get; set; } = 86400000; // 24 hours
}