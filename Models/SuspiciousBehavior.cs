using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Suspicious behavior pattern
/// </summary>
public class SuspiciousBehavior
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("behavior")]
    public string Behavior { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;
}