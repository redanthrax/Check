using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CheckWebAssembly.Models;

/// <summary>
/// Phishing indicator with pattern matching
/// </summary>
public class PhishingIndicator
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("pattern")]
    public string Pattern { get; set; } = string.Empty;

    [JsonPropertyName("flags")]
    public string Flags { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("context_required")]
    public List<string>? ContextRequired { get; set; }

    [JsonPropertyName("additional_checks")]
    public List<string>? AdditionalChecks { get; set; }

    [JsonIgnore]
    public Regex? CompiledPattern { get; set; }

    [JsonIgnore]
    public List<Regex>? CompiledContextPatterns { get; set; }

    [JsonIgnore]
    public List<Regex>? CompiledAdditionalChecks { get; set; }

    /// <summary>
    /// Compiles regex patterns for performance optimization
    /// </summary>
    public void CompilePatterns()
    {
        try
        {
            var options = RegexOptions.Compiled;
            if (Flags.Contains("i", StringComparison.OrdinalIgnoreCase))
                options |= RegexOptions.IgnoreCase;
            if (Flags.Contains("m", StringComparison.OrdinalIgnoreCase))
                options |= RegexOptions.Multiline;

            CompiledPattern = new Regex(Pattern, options);

            if (ContextRequired?.Count > 0)
            {
                CompiledContextPatterns = new List<Regex>();
                foreach (var pattern in ContextRequired)
                {
                    CompiledContextPatterns.Add(new Regex(pattern, options));
                }
            }

            if (AdditionalChecks?.Count > 0)
            {
                CompiledAdditionalChecks = new List<Regex>();
                foreach (var check in AdditionalChecks)
                {
                    CompiledAdditionalChecks.Add(new Regex(Regex.Escape(check), options));
                }
            }
        }
        catch (Exception)
        {
            CompiledPattern = null;
            CompiledContextPatterns = null;
            CompiledAdditionalChecks = null;
        }
    }
}