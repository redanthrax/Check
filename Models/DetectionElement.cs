using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CheckWebAssembly.Models;

/// <summary>
/// Detection element with compiled regex patterns for performance
/// </summary>
public class DetectionElement
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("pattern")]
    public string? Pattern { get; set; }

    [JsonPropertyName("patterns")]
    public List<string>? Patterns { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("weight")]
    public double Weight { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonIgnore]
    public Regex? CompiledPattern { get; set; }

    [JsonIgnore]
    public List<Regex>? CompiledPatterns { get; set; }

    /// <summary>
    /// Compiles regex patterns for performance optimization
    /// </summary>
    public void CompilePatterns()
    {
        try
        {
            if (!string.IsNullOrEmpty(Pattern))
            {
                CompiledPattern = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            if (Patterns?.Count > 0)
            {
                CompiledPatterns = new List<Regex>();
                foreach (var pattern in Patterns)
                {
                    CompiledPatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }
            }
        }
        catch (Exception)
        {
            CompiledPattern = null;
            CompiledPatterns = null;
        }
    }
}