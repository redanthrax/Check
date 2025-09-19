using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Rogue apps detection configuration
/// </summary>
public class RogueAppsDetection
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("source_url")]
    public string SourceUrl { get; set; } = string.Empty;

    [JsonPropertyName("cache_duration")]
    public long CacheDuration { get; set; } = 86400000; // 24 hours

    [JsonPropertyName("update_interval")]
    public long UpdateInterval { get; set; } = 43200000; // 12 hours

    [JsonPropertyName("detection_action")]
    public string DetectionAction { get; set; } = "warn";

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = "high";

    [JsonPropertyName("log_matches")]
    public bool LogMatches { get; set; } = true;

    [JsonPropertyName("auto_update")]
    public bool AutoUpdate { get; set; } = true;

    [JsonPropertyName("fallback_on_error")]
    public bool FallbackOnError { get; set; } = true;
}