namespace LegoTracker.Services.Media;

public interface IMediaDownloader
{
    /// <summary>Downloads <paramref name="sourceUrl"/> to RootPath/<paramref name="relativePath"/>, creating directories as needed.</summary>
    Task DownloadToAsync(string sourceUrl, string relativePath, CancellationToken ct = default);
}
