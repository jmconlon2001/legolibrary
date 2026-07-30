using LegoTracker.Data.Enums;

namespace LegoTracker.Services.Notifications;

public record MediaDownloadEvent(
    int LegoSetId,
    int InstructionManualId,
    DownloadStatus Status,
    string? ErrorMessage,
    DateTime TimestampUtc);
