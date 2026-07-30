using System.Threading.Channels;

namespace LegoTracker.Services.Media;

public class MediaDownloadQueue
{
    private readonly Channel<MediaDownloadRequest> _channel = Channel.CreateUnbounded<MediaDownloadRequest>();

    public ValueTask EnqueueAsync(MediaDownloadRequest request, CancellationToken ct = default) =>
        _channel.Writer.WriteAsync(request, ct);

    public IAsyncEnumerable<MediaDownloadRequest> ReadAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}
