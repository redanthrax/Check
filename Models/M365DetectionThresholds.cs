using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Microsoft 365 detection thresholds configuration
/// </summary>
public class M365DetectionThresholds
{
    [JsonPropertyName("minimum_primary_elements")]
    public int MinimumPrimaryElements { get; set; } = 1;

    [JsonPropertyName("minimum_total_weight")]
    public int MinimumTotalWeight { get; set; } = 4;

    [JsonPropertyName("minimum_elements_overall")]
    public int MinimumElementsOverall { get; set; } = 3;

    [JsonPropertyName("minimum_secondary_only_weight")]
    public int MinimumSecondaryOnlyWeight { get; set; } = 9;

    [JsonPropertyName("minimum_secondary_only_elements")]
    public int MinimumSecondaryOnlyElements { get; set; } = 7;
}