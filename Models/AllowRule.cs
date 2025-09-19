using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Allow rule for legitimate pages
/// </summary>
public class AllowRule
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

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;
}