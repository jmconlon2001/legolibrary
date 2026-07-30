using System.Text.Json.Serialization;

namespace LegoTracker.Services.Rebrickable.Dtos;

public class RebrickablePartDto
{
    [JsonPropertyName("part_num")]
    public string PartNum { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("part_cat_id")]
    public int? PartCatId { get; set; }

    [JsonPropertyName("part_img_url")]
    public string? PartImgUrl { get; set; }
}
