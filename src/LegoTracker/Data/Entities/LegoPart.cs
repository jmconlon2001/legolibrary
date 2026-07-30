namespace LegoTracker.Data.Entities;

public class LegoPart
{
    public required string PartNum { get; set; }
    public required string Name { get; set; }
    public int? PartCategoryId { get; set; }

    public PartCategory? PartCategory { get; set; }
    public List<SetInventory> SetInventories { get; } = [];
}
