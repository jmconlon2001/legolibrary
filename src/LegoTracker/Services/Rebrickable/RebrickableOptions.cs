namespace LegoTracker.Services.Rebrickable;

public class RebrickableOptions
{
    public const string SectionName = "Rebrickable";

    public required string ApiKey { get; set; }
    public string BaseUrl { get; set; } = "https://rebrickable.com/api/v3/lego/";
}
