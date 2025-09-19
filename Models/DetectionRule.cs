using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Generic detection rule
/// </summary>
public class DetectionRule
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("condition")]
    public Dictionary<string, object> Condition { get; set; } = new();

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}