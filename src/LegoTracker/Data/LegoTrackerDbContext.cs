using LegoTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LegoTracker.Data;

public class LegoTrackerDbContext(DbContextOptions<LegoTrackerDbContext> options) : DbContext(options)
{
    public DbSet<LegoSet> Sets => Set<LegoSet>();
    public DbSet<Theme> Themes => Set<Theme>();
    public DbSet<LegoColor> Colors => Set<LegoColor>();
    public DbSet<PartCategory> PartCategories => Set<PartCategory>();
    public DbSet<LegoPart> Parts => Set<LegoPart>();
    public DbSet<Minifig> Minifigs => Set<Minifig>();
    public DbSet<SetInventory> SetInventories => Set<SetInventory>();
    public DbSet<SetMinifig> SetMinifigs => Set<SetMinifig>();
    public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();
    public DbSet<SetInstructionManual> InstructionManuals => Set<SetInstructionManual>();
    public DbSet<MissingPart> MissingParts => Set<MissingPart>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LegoSet>(e =>
        {
            e.HasIndex(x => x.SetNum).IsUnique();
            e.Property(x => x.Msrp).HasPrecision(10, 2);
            e.Property(x => x.EstimatedValue).HasPrecision(10, 2);
            e.HasOne(x => x.Theme)
                .WithMany(t => t.Sets)
                .HasForeignKey(x => x.ThemeId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.StorageLocation)
                .WithMany(s => s.Sets)
                .HasForeignKey(x => x.StorageLocationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Theme>(e =>
        {
            // Id is Rebrickable's own theme_id (externally supplied), not DB-generated.
            e.Property(x => x.Id).ValueGeneratedNever();
            e.HasOne(x => x.ParentTheme)
                .WithMany(x => x.ChildThemes)
                .HasForeignKey(x => x.ParentThemeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LegoColor>()
            // Id is Rebrickable's own color id (e.g. 0 = Black), not DB-generated.
            .Property(x => x.Id).ValueGeneratedNever();

        modelBuilder.Entity<PartCategory>()
            // Id is Rebrickable's own part_cat_id, not DB-generated.
            .Property(x => x.Id).ValueGeneratedNever();

        modelBuilder.Entity<StorageLocation>()
            .HasOne(x => x.ParentLocation)
            .WithMany(x => x.ChildLocations)
            .HasForeignKey(x => x.ParentLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LegoPart>(e =>
        {
            e.HasKey(x => x.PartNum);
            e.HasOne(x => x.PartCategory)
                .WithMany()
                .HasForeignKey(x => x.PartCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Minifig>().HasKey(x => x.FigNum);

        modelBuilder.Entity<SetInventory>(e =>
        {
            e.HasIndex(x => new { x.LegoSetId, x.PartNum, x.ColorId, x.IsSpare });
            e.HasOne(x => x.LegoSet)
                .WithMany(s => s.Inventory)
                .HasForeignKey(x => x.LegoSetId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Part)
                .WithMany(p => p.SetInventories)
                .HasForeignKey(x => x.PartNum)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Color)
                .WithMany()
                .HasForeignKey(x => x.ColorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SetMinifig>(e =>
        {
            e.HasOne(x => x.LegoSet)
                .WithMany(s => s.Minifigs)
                .HasForeignKey(x => x.LegoSetId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Minifig)
                .WithMany(m => m.SetMinifigs)
                .HasForeignKey(x => x.FigNum)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SetInstructionManual>()
            .HasOne(x => x.LegoSet)
            .WithMany(s => s.InstructionManuals)
            .HasForeignKey(x => x.LegoSetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MissingPart>(e =>
        {
            e.HasOne(x => x.LegoSet)
                .WithMany(s => s.MissingParts)
                .HasForeignKey(x => x.LegoSetId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Part)
                .WithMany()
                .HasForeignKey(x => x.PartNum)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Color)
                .WithMany()
                .HasForeignKey(x => x.ColorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
