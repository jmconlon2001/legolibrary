namespace LegoTracker.Data.Entities;

public class Theme
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? ParentThemeId { get; set; }

    public Theme? ParentTheme { get; set; }
    public List<Theme> ChildThemes { get; } = [];
    public List<LegoSet> Sets { get; } = [];
}
