namespace LegoTracker.Services.Media;

public record MediaDownloadRequest(int InstructionManualId, int LegoSetId, string SourceUrl, string DestinationRelativePath);
