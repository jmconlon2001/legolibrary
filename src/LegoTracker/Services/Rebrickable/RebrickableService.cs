using System.Net;
using System.Net.Http.Json;
using LegoTracker.Services.Rebrickable.Dtos;

namespace LegoTracker.Services.Rebrickable;

public class RebrickableService(HttpClient httpClient) : IRebrickableService
{
    public async Task<RebrickableSetDto?> GetSetAsync(string setNum, CancellationToken ct = default)
    {
        var normalized = NormalizeSetNum(setNum);
        var response = await httpClient.GetAsync($"sets/{normalized}/", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RebrickableSetDto>(ct);
    }

    public async Task<RebrickableThemeDto?> GetThemeAsync(int themeId, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"themes/{themeId}/", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RebrickableThemeDto>(ct);
    }

    public async Task<List<RebrickableSetPartDto>> GetSetPartsAsync(string setNum, CancellationToken ct = default)
    {
        var normalized = NormalizeSetNum(setNum);
        return await GetAllPagesAsync<RebrickableSetPartDto>($"sets/{normalized}/parts/?page_size=1000", ct);
    }

    public async Task<List<RebrickableMinifigDto>> GetSetMinifigsAsync(string setNum, CancellationToken ct = default)
    {
        var normalized = NormalizeSetNum(setNum);
        return await GetAllPagesAsync<RebrickableMinifigDto>($"sets/{normalized}/minifigs/?page_size=1000", ct);
    }

    private async Task<List<TResult>> GetAllPagesAsync<TResult>(string requestUri, CancellationToken ct)
    {
        var results = new List<TResult>();
        string? next = requestUri;

        while (next is not null)
        {
            var response = await httpClient.GetAsync(next, ct);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                break;
            }

            response.EnsureSuccessStatusCode();
            var page = await response.Content.ReadFromJsonAsync<PagedResponse<TResult>>(ct);
            if (page is null)
            {
                break;
            }

            results.AddRange(page.Results);
            next = page.Next;
        }

        return results;
    }

    /// <summary>Accepts either "75192" or "75192-1"; Rebrickable requires the "-1" variant suffix.</summary>
    private static string NormalizeSetNum(string setNum)
    {
        return setNum.Contains('-') ? setNum : $"{setNum}-1";
    }
}
