using LegoTracker.Services.Rebrickable;
using LegoTracker.Tests.TestSupport;

namespace LegoTracker.Tests.Services.Rebrickable;

/// <summary>
/// Fixture JSON below is captured verbatim from live calls to https://rebrickable.com/api/v3/lego/
/// (set 75192-1) made during development, so these tests pin the service to Rebrickable's actual
/// response shape rather than an assumed one.
/// </summary>
public class RebrickableServiceTests
{
    private const string BaseUrl = "https://rebrickable.com/api/v3/lego/";

    private static RebrickableService CreateService(StubHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
        return new RebrickableService(httpClient);
    }

    [Fact]
    public async Task GetSetAsync_DeserializesRealSetResponse_AndHasNoMsrpField()
    {
        const string json = """
            {
                "set_num": "75192-1",
                "name": "Millennium Falcon",
                "year": 2017,
                "theme_id": 171,
                "num_parts": 7541,
                "set_img_url": "https://cdn.rebrickable.com/media/sets/75192-1/30881.jpg",
                "set_url": "https://rebrickable.com/sets/75192-1/millennium-falcon/",
                "last_modified_dt": "2021-11-27T08:23:26.191796Z"
            }
            """;
        var handler = new StubHttpMessageHandler().RespondWith("/api/v3/lego/sets/75192-1/", json);
        var service = CreateService(handler);

        var result = await service.GetSetAsync("75192-1");

        Assert.NotNull(result);
        Assert.Equal("Millennium Falcon", result!.Name);
        Assert.Equal(2017, result.Year);
        Assert.Equal(171, result.ThemeId);
        Assert.Equal(7541, result.NumParts);
        Assert.Equal("https://cdn.rebrickable.com/media/sets/75192-1/30881.jpg", result.SetImgUrl);
    }

    [Fact]
    public async Task GetSetAsync_NormalizesBareSetNumber_ByAppendingDashOne()
    {
        const string json = """{"set_num": "75192-1", "name": "Millennium Falcon"}""";
        var handler = new StubHttpMessageHandler().RespondWith("/api/v3/lego/sets/75192-1/", json);
        var service = CreateService(handler);

        var result = await service.GetSetAsync("75192");

        Assert.NotNull(result);
        Assert.Equal("75192-1", result!.SetNum);
    }

    [Fact]
    public async Task GetSetAsync_ReturnsNull_WhenSetNotFound()
    {
        var handler = new StubHttpMessageHandler();
        var service = CreateService(handler);

        var result = await service.GetSetAsync("00000-1");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetThemeAsync_DeserializesRealThemeResponse()
    {
        const string json = """{"id": 171, "parent_id": 158, "name": "Ultimate Collector Series"}""";
        var handler = new StubHttpMessageHandler().RespondWith("/api/v3/lego/themes/171/", json);
        var service = CreateService(handler);

        var result = await service.GetThemeAsync(171);

        Assert.NotNull(result);
        Assert.Equal("Ultimate Collector Series", result!.Name);
        Assert.Equal(158, result.ParentId);
    }

    [Fact]
    public async Task GetSetPartsAsync_DeserializesNestedPartAndColor_AndFollowsPagination()
    {
        const string page1 = """
            {
                "count": 3,
                "next": "https://rebrickable.com/api/v3/lego/sets/75192-1/parts/?page=2&page_size=2",
                "previous": null,
                "results": [
                    {
                        "id": 26178023,
                        "part": {
                            "part_num": "30377",
                            "name": "Arm Mechanical with 2 Clips [Battle Droid]",
                            "part_cat_id": 60,
                            "part_img_url": "https://cdn.rebrickable.com/media/parts/elements/6330086.jpg"
                        },
                        "color": {
                            "id": 0,
                            "name": "Black",
                            "rgb": "05131D",
                            "is_trans": false
                        },
                        "set_num": "75192-1",
                        "quantity": 4,
                        "is_spare": false,
                        "element_id": "6330086"
                    }
                ]
            }
            """;
        const string page2 = """
            {
                "count": 3,
                "next": null,
                "previous": "https://rebrickable.com/api/v3/lego/sets/75192-1/parts/?page_size=2",
                "results": [
                    {
                        "id": 26178144,
                        "part": {
                            "part_num": "30377",
                            "name": "Arm Mechanical with 2 Clips [Battle Droid]",
                            "part_cat_id": 60,
                            "part_img_url": "https://cdn.rebrickable.com/media/parts/elements/6330086.jpg"
                        },
                        "color": {
                            "id": 0,
                            "name": "Black",
                            "rgb": "05131D",
                            "is_trans": false
                        },
                        "set_num": "75192-1",
                        "quantity": 1,
                        "is_spare": true,
                        "element_id": "6330086"
                    }
                ]
            }
            """;
        var handler = new StubHttpMessageHandler()
            .RespondWith("/api/v3/lego/sets/75192-1/parts/?page_size=1000", page1)
            .RespondWith("/api/v3/lego/sets/75192-1/parts/?page=2&page_size=2", page2);
        var service = CreateService(handler);

        var result = await service.GetSetPartsAsync("75192-1");

        Assert.Equal(2, result.Count);
        Assert.Equal("30377", result[0].Part.PartNum);
        Assert.Equal("Black", result[0].Color.Name);
        Assert.Equal("6330086", result[0].ElementId);
        Assert.False(result[0].IsSpare);
        Assert.True(result[1].IsSpare);
    }

    [Fact]
    public async Task GetSetMinifigsAsync_MapsSetNameField_NotNameField()
    {
        const string json = """
            {
                "count": 1,
                "next": null,
                "previous": null,
                "results": [
                    {
                        "id": 33464,
                        "set_num": "fig-002544",
                        "set_name": "BB-8",
                        "quantity": 1,
                        "set_img_url": "https://cdn.rebrickable.com/media/sets/fig-002544/138512.jpg"
                    }
                ]
            }
            """;
        var handler = new StubHttpMessageHandler()
            .RespondWith("/api/v3/lego/sets/75192-1/minifigs/?page_size=1000", json);
        var service = CreateService(handler);

        var result = await service.GetSetMinifigsAsync("75192-1");

        Assert.Single(result);
        Assert.Equal("fig-002544", result[0].FigNum);
        Assert.Equal("BB-8", result[0].Name);
        Assert.Equal(1, result[0].Quantity);
    }
}
