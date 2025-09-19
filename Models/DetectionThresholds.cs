using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Detection thresholds for scoring
/// </summary>
public class DetectionThresholds
{
    [JsonPropertyName("legitimate")]
    public int Legitimate { get; set; } = 85;

    [JsonPropertyName("suspicious")]
    public int Suspicious { get; set; } = 55;

    [JsonPropertyName("phishing")]
    public int Phishing { get; set; } = 25;
}