using System.Text.Json.Serialization;

namespace LegoTracker.Services.Rebrickable.Dtos;

public class RebrickableColorDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("rgb")]
    public string? Rgb { get; set; }

    [JsonPropertyName("is_trans")]
    public bool IsTrans { get; set; }
}
