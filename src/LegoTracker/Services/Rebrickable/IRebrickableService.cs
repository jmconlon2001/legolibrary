using LegoTracker.Services.Rebrickable.Dtos;

namespace LegoTracker.Services.Rebrickable;

public interface IRebrickableService
{
    Task<RebrickableSetDto?> GetSetAsync(string setNum, CancellationToken ct = default);
    Task<RebrickableThemeDto?> GetThemeAsync(int themeId, CancellationToken ct = default);
    Task<List<RebrickableSetPartDto>> GetSetPartsAsync(string setNum, CancellationToken ct = default);
    Task<List<RebrickableMinifigDto>> GetSetMinifigsAsync(string setNum, CancellationToken ct = default);
}
