using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Exclusion system to prevent false positives
/// </summary>
public class ExclusionSystem
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("domain_patterns")]
    public List<string> DomainPatterns { get; set; } = new();

    [JsonPropertyName("context_indicators")]
    public ContextIndicators ContextIndicators { get; set; } = new();
}