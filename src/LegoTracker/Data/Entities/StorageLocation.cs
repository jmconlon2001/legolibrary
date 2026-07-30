namespace LegoTracker.Data.Entities;

public class StorageLocation
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? ParentLocationId { get; set; }
    public string? Notes { get; set; }

    public StorageLocation? ParentLocation { get; set; }
    public List<StorageLocation> ChildLocations { get; } = [];
    public List<LegoSet> Sets { get; } = [];
}
