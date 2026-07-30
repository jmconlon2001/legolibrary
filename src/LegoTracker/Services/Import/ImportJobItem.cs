using LegoTracker.Data.Enums;

namespace LegoTracker.Services.Import;

public record ImportJobItem(Guid BatchId, string SetNum, BuildStatus InitialStatus, int BatchTotal);
