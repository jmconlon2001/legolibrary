using LegoTracker.Components;
using LegoTracker.Data;
using LegoTracker.Services.Export;
using LegoTracker.Services.Import;
using LegoTracker.Services.Media;
using LegoTracker.Services.Notifications;
using LegoTracker.Services.Rebrickable;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

builder.Services.Configure<RebrickableOptions>(builder.Configuration.GetSection(RebrickableOptions.SectionName));
builder.Services.AddHttpClient<IRebrickableService, RebrickableService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<RebrickableOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"key {options.ApiKey}");
})
.AddPolicyHandler(RebrickablePolicies.GetRetryPolicy())
.AddPolicyHandler(RebrickablePolicies.GetCircuitBreakerPolicy());

builder.Services.Configure<MediaStorageOptions>(builder.Configuration.GetSection(MediaStorageOptions.SectionName));
builder.Services.AddHttpClient<IMediaDownloader, MediaDownloader>();
builder.Services.AddScoped<SetImportService>();

builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<ImportJobQueue>();
builder.Services.AddHostedService<ImportBackgroundService>();
builder.Services.AddSingleton<MediaDownloadQueue>();
builder.Services.AddHostedService<MediaDownloadBackgroundService>();

builder.Services.AddSingleton<CsvExportService>();
builder.Services.AddSingleton<WantedListExportService>();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'ConnectionStrings:Default' configuration.");
var dataSourceDirectory = Path.GetDirectoryName(new SqliteConnectionStringBuilder(connectionString).DataSource);
if (!string.IsNullOrEmpty(dataSourceDirectory))
{
    Directory.CreateDirectory(dataSourceDirectory);
}

builder.Services.AddDbContext<LegoTrackerDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LegoTrackerDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

var mediaRootPath = Path.GetFullPath(app.Configuration.GetSection(MediaStorageOptions.SectionName)["RootPath"] ?? "media");
Directory.CreateDirectory(mediaRootPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(mediaRootPath),
    RequestPath = "/media"
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/sets/{id:int}/missing-parts/export", async (
    int id, string format, LegoTrackerDbContext db, CsvExportService csvExport, WantedListExportService wantedListExport) =>
{
    var rows = await db.MissingParts
        .Where(m => m.LegoSetId == id)
        .Select(m => new MissingPartExportRow(m.PartNum, m.Part!.Name, m.Color!.Name, m.IssueType.ToString(), m.Quantity, m.Notes))
        .ToListAsync();

    var (content, fileName) = format switch
    {
        "wanted" => (wantedListExport.ExportWantedList(rows), $"set-{id}-wanted-list.csv"),
        _ => (csvExport.ExportMissingParts(rows), $"set-{id}-missing-parts.csv")
    };

    return Results.File(System.Text.Encoding.UTF8.GetBytes(content), "text/csv", fileName);
});

app.Run();
