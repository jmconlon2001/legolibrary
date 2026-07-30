namespace LegoTracker.Services.Notifications;

public class NotificationService : INotificationService
{
    public event Action<ImportProgressEvent>? OnImportProgress;
    public event Action<MediaDownloadEvent>? OnMediaDownloadChanged;

    public void PublishImportProgress(ImportProgressEvent e) => OnImportProgress?.Invoke(e);

    public void PublishMediaDownloadChanged(MediaDownloadEvent e) => OnMediaDownloadChanged?.Invoke(e);
}
