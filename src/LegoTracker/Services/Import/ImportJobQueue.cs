using System.Threading.Channels;

namespace LegoTracker.Services.Import;

public class ImportJobQueue
{
    private readonly Channel<ImportJobItem> _channel = Channel.CreateUnbounded<ImportJobItem>();

    public ValueTask EnqueueAsync(ImportJobItem item, CancellationToken ct = default) =>
        _channel.Writer.WriteAsync(item, ct);

    public IAsyncEnumerable<ImportJobItem> ReadAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}
