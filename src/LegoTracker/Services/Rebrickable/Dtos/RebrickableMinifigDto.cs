using System.Text.Json.Serialization;

namespace LegoTracker.Services.Rebrickable.Dtos;

/// <summary>
/// Mirrors GET /lego/sets/{set_num}/minifigs/. Rebrickable models minifigs internally as
/// "sets", so the fig number and name are returned under "set_num"/"set_name" even though
/// they identify a minifig (e.g. "fig-000001") rather than a buildable set. Verified live
/// against a real API key/response (set 75192-1) during M2.
/// </summary>
public class RebrickableMinifigDto
{
    [JsonPropertyName("set_num")]
    public string FigNum { get; set; } = null!;

    [JsonPropertyName("set_name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("set_img_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
