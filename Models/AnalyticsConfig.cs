namespace CheckWebAssembly.Models;

/// <summary>
/// Analytics configuration
/// </summary>
public class AnalyticsConfig
{
    public bool Enabled { get; set; } = false;
    public string TrackingId { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public bool AnonymizeData { get; set; } = true;
    public bool RespectDoNotTrack { get; set; } = true;
}