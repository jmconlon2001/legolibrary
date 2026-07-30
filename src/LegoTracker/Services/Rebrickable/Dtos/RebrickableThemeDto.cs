using System.Text.Json.Serialization;

namespace LegoTracker.Services.Rebrickable.Dtos;

/// <summary>Mirrors GET /lego/themes/{id}/. A "subtheme" is simply a Theme whose ParentId is set.</summary>
public class RebrickableThemeDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("parent_id")]
    public int? ParentId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
