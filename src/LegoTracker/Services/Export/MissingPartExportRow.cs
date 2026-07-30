namespace LegoTracker.Services.Export;

public record MissingPartExportRow(string? PartNum, string? PartName, string? ColorName, string IssueType, int Quantity, string? Notes);
