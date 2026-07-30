using LegoTracker.Data;
using LegoTracker.Data.Entities;
using LegoTracker.Data.Enums;
using LegoTracker.Services.Media;
using LegoTracker.Services.Rebrickable;
using Microsoft.EntityFrameworkCore;

namespace LegoTracker.Services.Import;

public class SetNotFoundException(string setNum) : Exception($"Set '{setNum}' was not found on Rebrickable.");

public class SetImportService(
    LegoTrackerDbContext db,
    IRebrickableService rebrickable,
    IMediaDownloader mediaDownloader,
    ILogger<SetImportService> logger)
{
    public async Task<int> ImportSetAsync(string setNum, BuildStatus initialStatus, CancellationToken ct = default)
    {
        var setDto = await rebrickable.GetSetAsync(setNum, ct) ?? throw new SetNotFoundException(setNum);

        int? themeId = null;
        if (setDto.ThemeId is { } rebrickableThemeId)
        {
            themeId = await GetOrCreateThemeAsync(rebrickableThemeId, ct);
        }

        var legoSet = await db.Sets.FirstOrDefaultAsync(s => s.SetNum == setDto.SetNum, ct);
        var isNew = legoSet is null;
        if (legoSet is null)
        {
            legoSet = new LegoSet { SetNum = setDto.SetNum, Name = setDto.Name, BuildStatus = initialStatus };
            db.Sets.Add(legoSet);
        }

        legoSet.Name = setDto.Name;
        legoSet.ThemeId = themeId;
        legoSet.Year = setDto.Year;
        legoSet.PieceCount = setDto.NumParts;
        legoSet.BoxArtSourceUrl = setDto.SetImgUrl;
        legoSet.UpdatedAtUtc = DateTime.UtcNow;
        if (isNew)
        {
            legoSet.BuildStatus = initialStatus;
        }

        await db.SaveChangesAsync(ct);

        await ImportInventoryAsync(legoSet, ct);
        await ImportMinifigsAsync(legoSet, ct);
        await DownloadBoxArtAsync(legoSet, ct);

        await db.SaveChangesAsync(ct);
        return legoSet.Id;
    }

    private async Task<int> GetOrCreateThemeAsync(int rebrickableThemeId, CancellationToken ct)
    {
        var existing = await db.Themes.FindAsync([rebrickableThemeId], ct);
        if (existing is not null)
        {
            return existing.Id;
        }

        var themeDto = await rebrickable.GetThemeAsync(rebrickableThemeId, ct);
        var theme = new Theme
        {
            Id = rebrickableThemeId,
            Name = themeDto?.Name ?? $"Theme {rebrickableThemeId}",
            ParentThemeId = themeDto?.ParentId is { } parentId && parentId != rebrickableThemeId
                ? await GetOrCreateThemeAsync(parentId, ct)
                : null
        };
        db.Themes.Add(theme);
        await db.SaveChangesAsync(ct);
        return theme.Id;
    }

    private async Task ImportInventoryAsync(LegoSet legoSet, CancellationToken ct)
    {
        var setParts = await rebrickable.GetSetPartsAsync(legoSet.SetNum, ct);

        var existingInventory = await db.SetInventories.Where(i => i.LegoSetId == legoSet.Id).ToListAsync(ct);
        db.SetInventories.RemoveRange(existingInventory);

        foreach (var item in setParts)
        {
            await GetOrCreatePartCategoryAsync(item.Part.PartCatId, ct);
            await GetOrCreatePartAsync(item.Part, ct);
            await GetOrCreateColorAsync(item.Color, ct);

            db.SetInventories.Add(new SetInventory
            {
                LegoSetId = legoSet.Id,
                PartNum = item.Part.PartNum,
                ColorId = item.Color.Id,
                ElementId = item.ElementId,
                Quantity = item.Quantity,
                IsSpare = item.IsSpare,
                ImageUrl = item.Part.PartImgUrl
            });
        }
    }

    private async Task ImportMinifigsAsync(LegoSet legoSet, CancellationToken ct)
    {
        var minifigs = await rebrickable.GetSetMinifigsAsync(legoSet.SetNum, ct);

        var existingMinifigs = await db.SetMinifigs.Where(m => m.LegoSetId == legoSet.Id).ToListAsync(ct);
        db.SetMinifigs.RemoveRange(existingMinifigs);

        foreach (var item in minifigs)
        {
            var minifig = await db.Minifigs.FindAsync([item.FigNum], ct);
            if (minifig is null)
            {
                minifig = new Minifig { FigNum = item.FigNum, Name = item.Name, ImageUrl = item.ImageUrl };
                db.Minifigs.Add(minifig);
            }

            db.SetMinifigs.Add(new SetMinifig
            {
                LegoSetId = legoSet.Id,
                FigNum = item.FigNum,
                Quantity = item.Quantity
            });
        }
    }

    private async Task GetOrCreatePartCategoryAsync(int? partCatId, CancellationToken ct)
    {
        if (partCatId is null || await db.PartCategories.FindAsync([partCatId.Value], ct) is not null)
        {
            return;
        }

        db.PartCategories.Add(new PartCategory { Id = partCatId.Value, Name = $"Category {partCatId.Value}" });
    }

    private async Task GetOrCreatePartAsync(Rebrickable.Dtos.RebrickablePartDto partDto, CancellationToken ct)
    {
        var existing = await db.Parts.FindAsync([partDto.PartNum], ct);
        if (existing is not null)
        {
            return;
        }

        db.Parts.Add(new LegoPart
        {
            PartNum = partDto.PartNum,
            Name = partDto.Name,
            PartCategoryId = partDto.PartCatId
        });
    }

    private async Task GetOrCreateColorAsync(Rebrickable.Dtos.RebrickableColorDto colorDto, CancellationToken ct)
    {
        var existing = await db.Colors.FindAsync([colorDto.Id], ct);
        if (existing is not null)
        {
            return;
        }

        db.Colors.Add(new LegoColor
        {
            Id = colorDto.Id,
            Name = colorDto.Name,
            RgbHex = colorDto.Rgb,
            IsTrans = colorDto.IsTrans
        });
    }

    private async Task DownloadBoxArtAsync(LegoSet legoSet, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(legoSet.BoxArtSourceUrl))
        {
            return;
        }

        var extension = Path.GetExtension(new Uri(legoSet.BoxArtSourceUrl).AbsolutePath) is { Length: > 0 } ext ? ext : ".jpg";
        var relativePath = $"boxart/{legoSet.SetNum}{extension}";

        try
        {
            await mediaDownloader.DownloadToAsync(legoSet.BoxArtSourceUrl, relativePath, ct);
            legoSet.BoxArtLocalPath = relativePath;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to download box art for set {SetNum}", legoSet.SetNum);
        }
    }
}
