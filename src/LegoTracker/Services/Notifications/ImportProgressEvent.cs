namespace LegoTracker.Services.Notifications;

public record ImportProgressEvent(
    Guid BatchId,
    int Processed,
    int Total,
    string SetNum,
    string Message,
    bool IsError,
    DateTime TimestampUtc);
