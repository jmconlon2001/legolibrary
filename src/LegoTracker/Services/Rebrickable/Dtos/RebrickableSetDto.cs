using System.Text.Json.Serialization;

namespace LegoTracker.Services.Rebrickable.Dtos;

/// <summary>Mirrors GET /lego/sets/{set_num}/. Rebrickable does not provide MSRP/pricing data.</summary>
public class RebrickableSetDto
{
    [JsonPropertyName("set_num")]
    public string SetNum { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("theme_id")]
    public int? ThemeId { get; set; }

    [JsonPropertyName("num_parts")]
    public int? NumParts { get; set; }

    [JsonPropertyName("set_img_url")]
    public string? SetImgUrl { get; set; }

    [JsonPropertyName("set_url")]
    public string? SetUrl { get; set; }
}
