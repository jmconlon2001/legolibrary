using System.Collections.Concurrent;
using LegoTracker.Services.Notifications;

namespace LegoTracker.Services.Import;

/// <summary>
/// Processes both single-set adds and bulk-import batches through one queue, sequentially
/// (degree of parallelism 1) to avoid hammering Rebrickable. A failed item is logged and
/// reported via <see cref="INotificationService"/> without halting the rest of the batch.
/// </summary>
public class ImportBackgroundService(
    ImportJobQueue queue,
    IServiceScopeFactory scopeFactory,
    INotificationService notifications,
    ILogger<ImportBackgroundService> logger) : BackgroundService
{
    private readonly ConcurrentDictionary<Guid, int> _processedByBatch = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in queue.ReadAllAsync(stoppingToken))
        {
            string message;
            var isError = false;

            try
            {
                using var scope = scopeFactory.CreateScope();
                var importService = scope.ServiceProvider.GetRequiredService<SetImportService>();
                await importService.ImportSetAsync(item.SetNum, item.InitialStatus, stoppingToken);
                message = $"Imported {item.SetNum}";
            }
            catch (Exception ex)
            {
                isError = true;
                message = $"Failed to import {item.SetNum}: {ex.Message}";
                logger.LogWarning(ex, "Import failed for set {SetNum}", item.SetNum);
            }

            var processed = _processedByBatch.AddOrUpdate(item.BatchId, 1, (_, count) => count + 1);
            if (processed >= item.BatchTotal)
            {
                _processedByBatch.TryRemove(item.BatchId, out _);
            }

            notifications.PublishImportProgress(new ImportProgressEvent(
                item.BatchId, processed, item.BatchTotal, item.SetNum, message, isError, DateTime.UtcNow));
        }
    }
}
