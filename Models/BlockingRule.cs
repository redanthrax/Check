using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Blocking rule for malicious pages
/// </summary>
public class BlockingRule
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("condition")]
    public Dictionary<string, object> Condition { get; set; } = new();

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;
}