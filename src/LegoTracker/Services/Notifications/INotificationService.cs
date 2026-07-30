namespace LegoTracker.Services.Notifications;

/// <summary>
/// Plain C# event pub/sub for pushing background-job progress into Blazor Server components.
/// Blazor Server already runs each connected component over its own persistent SignalR circuit,
/// so a second explicit SignalR hub would duplicate that transport for no benefit. Components
/// subscribe here and marshal onto their own circuit via InvokeAsync(StateHasChanged).
/// </summary>
public interface INotificationService
{
    event Action<ImportProgressEvent>? OnImportProgress;
    event Action<MediaDownloadEvent>? OnMediaDownloadChanged;

    void PublishImportProgress(ImportProgressEvent e);
    void PublishMediaDownloadChanged(MediaDownloadEvent e);
}
