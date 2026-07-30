using LegoTracker.Data;
using LegoTracker.Data.Enums;
using LegoTracker.Services.Notifications;
using Microsoft.EntityFrameworkCore;

namespace LegoTracker.Services.Media;

/// <summary>
/// Downloads instruction manual PDFs the user has pasted a URL for, caching them locally under
/// the media root so the viewer never hotlinks the original source. Publishes each status
/// transition (Downloading -> Completed/Failed) so the Instructions page can update live.
/// </summary>
public class MediaDownloadBackgroundService(
    MediaDownloadQueue queue,
    IServiceScopeFactory scopeFactory,
    INotificationService notifications,
    ILogger<MediaDownloadBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in queue.ReadAllAsync(stoppingToken))
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LegoTrackerDbContext>();
            var downloader = scope.ServiceProvider.GetRequiredService<IMediaDownloader>();

            var manual = await db.InstructionManuals.FindAsync([request.InstructionManualId], stoppingToken);
            if (manual is null)
            {
                continue;
            }

            manual.DownloadStatus = DownloadStatus.Downloading;
            await db.SaveChangesAsync(stoppingToken);
            notifications.PublishMediaDownloadChanged(new MediaDownloadEvent(
                request.LegoSetId, request.InstructionManualId, DownloadStatus.Downloading, null, DateTime.UtcNow));

            try
            {
                await downloader.DownloadToAsync(request.SourceUrl, request.DestinationRelativePath, stoppingToken);
                manual.LocalFilePath = request.DestinationRelativePath;
                manual.DownloadStatus = DownloadStatus.Completed;
                manual.DownloadedAtUtc = DateTime.UtcNow;
                manual.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                manual.DownloadStatus = DownloadStatus.Failed;
                manual.ErrorMessage = ex.Message;
                logger.LogWarning(ex, "Failed to download instruction manual {ManualId}", request.InstructionManualId);
            }

            await db.SaveChangesAsync(stoppingToken);
            notifications.PublishMediaDownloadChanged(new MediaDownloadEvent(
                request.LegoSetId, request.InstructionManualId, manual.DownloadStatus, manual.ErrorMessage, DateTime.UtcNow));
        }
    }
}
