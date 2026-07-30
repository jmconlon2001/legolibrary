namespace LegoTracker.Data.Entities;

public class SetMinifig
{
    public int Id { get; set; }
    public int LegoSetId { get; set; }
    public required string FigNum { get; set; }
    public int Quantity { get; set; }

    public LegoSet? LegoSet { get; set; }
    public Minifig? Minifig { get; set; }
}
