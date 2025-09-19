using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CheckWebAssembly.Models;

/// <summary>
/// Resource validation rule model with compiled patterns
/// </summary>
public class ResourceValidationRule
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("pattern")]
    public string Pattern { get; set; } = string.Empty;

    [JsonPropertyName("required_origins")]
    public List<string> RequiredOrigins { get; set; } = new();

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonIgnore]
    public Regex? CompiledPattern { get; set; }

    /// <summary>
    /// Compiles pattern for performance optimization
    /// </summary>
    public void CompilePattern()
    {
        try
        {
            if (!string.IsNullOrEmpty(Pattern))
            {
                CompiledPattern = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }
        catch (Exception)
        {
            CompiledPattern = null;
        }
    }
}