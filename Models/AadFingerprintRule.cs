using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// AAD fingerprint rule model
/// </summary>
public class AadFingerprintRule
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("condition")]
    public string Condition { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("weight")]
    public int Weight { get; set; }
}