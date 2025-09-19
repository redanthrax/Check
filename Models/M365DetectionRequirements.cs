using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Microsoft 365 detection requirements with primary and secondary categorization
/// </summary>
public class M365DetectionRequirements
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("primary_elements")]
    public List<DetectionElement> PrimaryElements { get; set; } = new();

    [JsonPropertyName("secondary_elements")]
    public List<DetectionElement> SecondaryElements { get; set; } = new();

    [JsonPropertyName("detection_thresholds")]
    public M365DetectionThresholds DetectionThresholds { get; set; } = new();

    [JsonPropertyName("legacy_minimum_required")]
    public int LegacyMinimumRequired { get; set; } = 4;

    [JsonPropertyName("legacy_all_must_be_present")]
    public bool LegacyAllMustBePresent { get; set; } = false;
}