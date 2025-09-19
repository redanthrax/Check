using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CheckWebAssembly.Models;

/// <summary>
/// Legitimate pattern for known good sites
/// </summary>
public class LegitimatePattern
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("pattern")]
    public string? Pattern { get; set; }

    [JsonPropertyName("element_selectors")]
    public List<string>? ElementSelectors { get; set; }

    [JsonPropertyName("content_patterns")]
    public List<string>? ContentPatterns { get; set; }

    [JsonPropertyName("resource_patterns")]
    public List<string>? ResourcePatterns { get; set; }

    [JsonPropertyName("csp_domains")]
    public List<string>? CspDomains { get; set; }

    [JsonPropertyName("referrer_patterns")]
    public List<string>? ReferrerPatterns { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonIgnore]
    public Regex? CompiledPattern { get; set; }

    [JsonIgnore]
    public List<Regex>? CompiledContentPatterns { get; set; }

    [JsonIgnore]
    public List<Regex>? CompiledResourcePatterns { get; set; }

    [JsonIgnore]
    public List<Regex>? CompiledReferrerPatterns { get; set; }

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

            if (ContentPatterns?.Count > 0)
            {
                CompiledContentPatterns = new List<Regex>();
                foreach (var pattern in ContentPatterns)
                {
                    CompiledContentPatterns.Add(new Regex(Regex.Escape(pattern), RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }
            }

            if (ResourcePatterns?.Count > 0)
            {
                CompiledResourcePatterns = new List<Regex>();
                foreach (var pattern in ResourcePatterns)
                {
                    CompiledResourcePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }
            }

            if (ReferrerPatterns?.Count > 0)
            {
                CompiledReferrerPatterns = new List<Regex>();
                foreach (var pattern in ReferrerPatterns)
                {
                    CompiledReferrerPatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }
            }
        }
        catch (Exception)
        {
            CompiledPattern = null;
            CompiledContentPatterns = null;
            CompiledResourcePatterns = null;
            CompiledReferrerPatterns = null;
        }
    }
}