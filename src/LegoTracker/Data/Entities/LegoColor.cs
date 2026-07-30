namespace LegoTracker.Data.Entities;

public class LegoColor
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? RgbHex { get; set; }
    public bool IsTrans { get; set; }
}
