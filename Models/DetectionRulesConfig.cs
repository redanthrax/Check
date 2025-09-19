using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Root detection rules configuration
/// </summary>
public class DetectionRulesConfig
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";

    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("trusted_login_patterns")]
    public List<string> TrustedLoginPatterns { get; set; } = new();

    [JsonPropertyName("microsoft_domain_patterns")]
    public List<string> MicrosoftDomainPatterns { get; set; } = new();

    [JsonPropertyName("exclusion_system")]
    public ExclusionSystem ExclusionSystem { get; set; } = new();

    [JsonPropertyName("legitimate_discussion_domains")]
    public List<string> LegitimateDiscussionDomains { get; set; } = new();

    [JsonPropertyName("m365_detection_requirements")]
    public M365DetectionRequirements M365DetectionRequirements { get; set; } = new();

    [JsonPropertyName("blocking_rules")]
    public List<BlockingRule> BlockingRules { get; set; } = new();

    [JsonPropertyName("allow_rules")]
    public List<AllowRule> AllowRules { get; set; } = new();

    [JsonPropertyName("aad_detection_elements")]
    public List<AadDetectionElement> AadDetectionElements { get; set; } = new();

    [JsonPropertyName("required_elements")]
    public List<RequiredElement> RequiredElements { get; set; } = new();

    [JsonPropertyName("rules")]
    public List<DetectionRule> Rules { get; set; } = new();

    [JsonPropertyName("thresholds")]
    public DetectionThresholds Thresholds { get; set; } = new();

    [JsonPropertyName("phishing_indicators")]
    public List<PhishingIndicator> PhishingIndicators { get; set; } = new();

    [JsonPropertyName("legitimate_patterns")]
    public List<LegitimatePattern> LegitimatePatterns { get; set; } = new();

    [JsonPropertyName("suspicious_behaviors")]
    public List<SuspiciousBehavior> SuspiciousBehaviors { get; set; } = new();

    [JsonPropertyName("detection_settings")]
    public DetectionSettings DetectionSettings { get; set; } = new();

    [JsonPropertyName("detection_logic")]
    public DetectionLogic DetectionLogic { get; set; } = new();

    [JsonPropertyName("rogue_apps_detection")]
    public RogueAppsDetection RogueAppsDetection { get; set; } = new();
}