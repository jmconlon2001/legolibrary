namespace LegoTracker.Data.Entities;

public class SetInventory
{
    public int Id { get; set; }
    public int LegoSetId { get; set; }
    public required string PartNum { get; set; }
    public int ColorId { get; set; }
    public string? ElementId { get; set; }
    public int Quantity { get; set; }
    public bool IsSpare { get; set; }
    public string? ImageUrl { get; set; }

    public LegoSet? LegoSet { get; set; }
    public LegoPart? Part { get; set; }
    public LegoColor? Color { get; set; }
}
