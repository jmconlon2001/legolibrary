using LegoTracker.Data.Enums;

namespace LegoTracker.Data.Entities;

public class LegoSet
{
    public int Id { get; set; }
    public required string SetNum { get; set; }
    public required string Name { get; set; }
    public int? ThemeId { get; set; }
    public int? Year { get; set; }
    public int? PieceCount { get; set; }
    public decimal? Msrp { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string? BoxArtLocalPath { get; set; }
    public string? BoxArtSourceUrl { get; set; }
    public BuildStatus BuildStatus { get; set; } = BuildStatus.InBox;
    public int? StorageLocationId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Theme? Theme { get; set; }
    public StorageLocation? StorageLocation { get; set; }
    public List<SetInventory> Inventory { get; } = [];
    public List<SetMinifig> Minifigs { get; } = [];
    public List<SetInstructionManual> InstructionManuals { get; } = [];
    public List<MissingPart> MissingParts { get; } = [];
}
