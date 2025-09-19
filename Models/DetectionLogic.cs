using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Detection logic configuration
/// </summary>
public class DetectionLogic
{
    [JsonPropertyName("aad_fingerprint_rules")]
    public List<AadFingerprintRule> AadFingerprintRules { get; set; } = new();

    [JsonPropertyName("trigger_rules")]
    public List<TriggerRule> TriggerRules { get; set; } = new();

    [JsonPropertyName("form_validation_rules")]
    public List<FormValidationRule> FormValidationRules { get; set; } = new();

    [JsonPropertyName("resource_validation_rules")]
    public List<ResourceValidationRule> ResourceValidationRules { get; set; } = new();
}