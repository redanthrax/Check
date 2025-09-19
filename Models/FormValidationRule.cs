using System.Text.Json.Serialization;

namespace CheckWebAssembly.Models;

/// <summary>
/// Form validation rule model
/// </summary>
public class FormValidationRule
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("condition")]
    public string Condition { get; set; } = string.Empty;

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}