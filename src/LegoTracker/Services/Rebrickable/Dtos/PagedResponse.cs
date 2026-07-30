using System.Text.Json.Serialization;

namespace LegoTracker.Services.Rebrickable.Dtos;

public class PagedResponse<TResult>
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("previous")]
    public string? Previous { get; set; }

    [JsonPropertyName("results")]
    public List<TResult> Results { get; set; } = [];
}
