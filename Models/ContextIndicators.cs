using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Context indicators for exclusion system
/// </summary>
public class ContextIndicators
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("legitimate_contexts")]
    public List<string> LegitimateContexts { get; set; } = new();

    [JsonPropertyName("legitimate_sso_patterns")]
    public List<string> LegitimateSSoPatterns { get; set; } = new();

    [JsonPropertyName("suspicious_contexts")]
    public List<string> SuspiciousContexts { get; set; } = new();
}