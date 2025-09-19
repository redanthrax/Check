using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CheckWebAssembly.Models;

/// <summary>
/// Azure AD detection element
/// </summary>
public class AadDetectionElement
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("selectors")]
    public List<string>? Selectors { get; set; }

    [JsonPropertyName("text_patterns")]
    public List<string>? TextPatterns { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonIgnore]
    public List<Regex>? CompiledTextPatterns { get; set; }

    /// <summary>
    /// Compiles text patterns for performance optimization
    /// </summary>
    public void CompileTextPatterns()
    {
        try
        {
            if (TextPatterns?.Count > 0)
            {
                CompiledTextPatterns = new List<Regex>();
                foreach (var pattern in TextPatterns)
                {
                    CompiledTextPatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }
            }
        }
        catch (Exception)
        {
            CompiledTextPatterns = null;
        }
    }
}