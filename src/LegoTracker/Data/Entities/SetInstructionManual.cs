using LegoTracker.Data.Enums;

namespace LegoTracker.Data.Entities;

public class SetInstructionManual
{
    public int Id { get; set; }
    public int LegoSetId { get; set; }
    public required string SourceUrl { get; set; }
    public string? LocalFilePath { get; set; }
    public DownloadStatus DownloadStatus { get; set; } = DownloadStatus.Pending;
    public DateTime? DownloadedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Label { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public LegoSet? LegoSet { get; set; }
}
