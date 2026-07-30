using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegoTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RgbHex = table.Column<string>(type: "TEXT", nullable: true),
                    IsTrans = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Minifigs",
                columns: table => new
                {
                    FigNum = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Minifigs", x => x.FigNum);
                });

            migrationBuilder.CreateTable(
                name: "PartCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentLocationId = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageLocations_StorageLocations_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentThemeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Themes_Themes_ParentThemeId",
                        column: x => x.ParentThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    PartNum = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PartCategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.PartNum);
                    table.ForeignKey(
                        name: "FK_Parts_PartCategories_PartCategoryId",
                        column: x => x.PartCategoryId,
                        principalTable: "PartCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SetNum = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ThemeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    PieceCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Msrp = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    BoxArtLocalPath = table.Column<string>(type: "TEXT", nullable: true),
                    BoxArtSourceUrl = table.Column<string>(type: "TEXT", nullable: true),
                    BuildStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    StorageLocationId = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sets_StorageLocations_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Sets_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InstructionManuals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LegoSetId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceUrl = table.Column<string>(type: "TEXT", nullable: false),
                    LocalFilePath = table.Column<string>(type: "TEXT", nullable: true),
                    DownloadStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    DownloadedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    Label = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructionManuals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructionManuals_Sets_LegoSetId",
                        column: x => x.LegoSetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissingParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LegoSetId = table.Column<int>(type: "INTEGER", nullable: false),
                    PartNum = table.Column<string>(type: "TEXT", nullable: true),
                    ColorId = table.Column<int>(type: "INTEGER", nullable: true),
                    IssueType = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ResolvedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissingParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissingParts_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MissingParts_Parts_PartNum",
                        column: x => x.PartNum,
                        principalTable: "Parts",
                        principalColumn: "PartNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MissingParts_Sets_LegoSetId",
                        column: x => x.LegoSetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LegoSetId = table.Column<int>(type: "INTEGER", nullable: false),
                    PartNum = table.Column<string>(type: "TEXT", nullable: false),
                    ColorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ElementId = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    IsSpare = table.Column<bool>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetInventories_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetInventories_Parts_PartNum",
                        column: x => x.PartNum,
                        principalTable: "Parts",
                        principalColumn: "PartNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetInventories_Sets_LegoSetId",
                        column: x => x.LegoSetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetMinifigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LegoSetId = table.Column<int>(type: "INTEGER", nullable: false),
                    FigNum = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetMinifigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetMinifigs_Minifigs_FigNum",
                        column: x => x.FigNum,
                        principalTable: "Minifigs",
                        principalColumn: "FigNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetMinifigs_Sets_LegoSetId",
                        column: x => x.LegoSetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructionManuals_LegoSetId",
                table: "InstructionManuals",
                column: "LegoSetId");

            migrationBuilder.CreateIndex(
                name: "IX_MissingParts_ColorId",
                table: "MissingParts",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_MissingParts_LegoSetId",
                table: "MissingParts",
                column: "LegoSetId");

            migrationBuilder.CreateIndex(
                name: "IX_MissingParts_PartNum",
                table: "MissingParts",
                column: "PartNum");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_PartCategoryId",
                table: "Parts",
                column: "PartCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SetInventories_ColorId",
                table: "SetInventories",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_SetInventories_LegoSetId_PartNum_ColorId_IsSpare",
                table: "SetInventories",
                columns: new[] { "LegoSetId", "PartNum", "ColorId", "IsSpare" });

            migrationBuilder.CreateIndex(
                name: "IX_SetInventories_PartNum",
                table: "SetInventories",
                column: "PartNum");

            migrationBuilder.CreateIndex(
                name: "IX_SetMinifigs_FigNum",
                table: "SetMinifigs",
                column: "FigNum");

            migrationBuilder.CreateIndex(
                name: "IX_SetMinifigs_LegoSetId",
                table: "SetMinifigs",
                column: "LegoSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_SetNum",
                table: "Sets",
                column: "SetNum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sets_StorageLocationId",
                table: "Sets",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_ThemeId",
                table: "Sets",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_ParentLocationId",
                table: "StorageLocations",
                column: "ParentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_ParentThemeId",
                table: "Themes",
                column: "ParentThemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructionManuals");

            migrationBuilder.DropTable(
                name: "MissingParts");

            migrationBuilder.DropTable(
                name: "SetInventories");

            migrationBuilder.DropTable(
                name: "SetMinifigs");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "Minifigs");

            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "PartCategories");

            migrationBuilder.DropTable(
                name: "StorageLocations");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
