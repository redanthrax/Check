using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Detection settings configuration
/// </summary>
public class DetectionSettings
{
    [JsonPropertyName("enable_real_time_scanning")]
    public bool EnableRealTimeScanning { get; set; } = true;

    [JsonPropertyName("enable_form_monitoring")]
    public bool EnableFormMonitoring { get; set; } = true;

    [JsonPropertyName("enable_url_verification")]
    public bool EnableUrlVerification { get; set; } = true;

    [JsonPropertyName("enable_content_analysis")]
    public bool EnableContentAnalysis { get; set; } = true;

    [JsonPropertyName("enable_verification_badge")]
    public bool EnableVerificationBadge { get; set; } = false;

    [JsonPropertyName("block_threshold")]
    public double BlockThreshold { get; set; } = 0.8;

    [JsonPropertyName("warn_threshold")]
    public double WarnThreshold { get; set; } = 0.6;

    [JsonPropertyName("monitor_threshold")]
    public double MonitorThreshold { get; set; } = 0.4;

    [JsonPropertyName("aad_detection_threshold")]
    public int AadDetectionThreshold { get; set; } = 2;

    [JsonPropertyName("required_elements_threshold")]
    public int RequiredElementsThreshold { get; set; } = 3;

    [JsonPropertyName("monitoring_timeout")]
    public int MonitoringTimeout { get; set; } = 20000;
}