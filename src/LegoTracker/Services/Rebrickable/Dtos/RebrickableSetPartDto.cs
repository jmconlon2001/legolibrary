using System.Text.Json.Serialization;

namespace LegoTracker.Services.Rebrickable.Dtos;

/// <summary>Mirrors one item of GET /lego/sets/{set_num}/parts/.</summary>
public class RebrickableSetPartDto
{
    [JsonPropertyName("part")]
    public RebrickablePartDto Part { get; set; } = null!;

    [JsonPropertyName("color")]
    public RebrickableColorDto Color { get; set; } = null!;

    [JsonPropertyName("element_id")]
    public string? ElementId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("is_spare")]
    public bool IsSpare { get; set; }
}
