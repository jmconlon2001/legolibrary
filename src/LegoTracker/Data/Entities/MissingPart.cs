using LegoTracker.Data.Enums;

namespace LegoTracker.Data.Entities;

public class MissingPart
{
    public int Id { get; set; }
    public int LegoSetId { get; set; }
    public string? PartNum { get; set; }
    public int? ColorId { get; set; }
    public IssueType IssueType { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Notes { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public LegoSet? LegoSet { get; set; }
    public LegoPart? Part { get; set; }
    public LegoColor? Color { get; set; }
}
