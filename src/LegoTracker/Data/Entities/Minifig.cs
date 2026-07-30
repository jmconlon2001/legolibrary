namespace LegoTracker.Data.Entities;

public class Minifig
{
    public required string FigNum { get; set; }
    public required string Name { get; set; }
    public string? ImageUrl { get; set; }

    public List<SetMinifig> SetMinifigs { get; } = [];
}
