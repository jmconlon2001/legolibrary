using Microsoft.Extensions.Options;

namespace LegoTracker.Services.Media;

public class MediaDownloader(HttpClient httpClient, IOptions<MediaStorageOptions> options) : IMediaDownloader
{
    public async Task DownloadToAsync(string sourceUrl, string relativePath, CancellationToken ct = default)
    {
        var destinationPath = Path.Combine(options.Value.RootPath, relativePath);
        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var response = await httpClient.GetAsync(sourceUrl, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        await using var sourceStream = await response.Content.ReadAsStreamAsync(ct);
        await using var fileStream = File.Create(destinationPath);
        await sourceStream.CopyToAsync(fileStream, ct);
    }
}
