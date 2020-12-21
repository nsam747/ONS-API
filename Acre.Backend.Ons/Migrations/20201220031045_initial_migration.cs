using Microsoft.EntityFrameworkCore.Migrations;

namespace Acre.Backend.Ons.Migrations
{
    public partial class initial_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subcategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ons_By_Age",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubcategoryId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    UpperBoundAge = table.Column<int>(nullable: false),
                    LowerBoundAge = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ons_By_Age", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ons_By_Age_Subcategories_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ons_By_Composition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubcategoryId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    AdultCount = table.Column<int>(nullable: false),
                    DependantCount = table.Column<int>(nullable: false),
                    EmploymentStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ons_By_Composition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ons_By_Composition_Subcategories_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ons_By_Region",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubcategoryId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    RegionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ons_By_Region", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ons_By_Region_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ons_By_Region_Subcategories_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ons_By_Age_SubcategoryId",
                table: "Ons_By_Age",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ons_By_Age_UpperBoundAge_LowerBoundAge",
                table: "Ons_By_Age",
                columns: new[] { "UpperBoundAge", "LowerBoundAge" });

            migrationBuilder.CreateIndex(
                name: "IX_Ons_By_Composition_SubcategoryId",
                table: "Ons_By_Composition",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ons_By_Composition_EmploymentStatus_DependantCount_AdultCount",
                table: "Ons_By_Composition",
                columns: new[] { "EmploymentStatus", "DependantCount", "AdultCount" });

            migrationBuilder.CreateIndex(
                name: "IX_Ons_By_Region_RegionId",
                table: "Ons_By_Region",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ons_By_Region_SubcategoryId",
                table: "Ons_By_Region",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategories_CategoryId",
                table: "Subcategories",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ons_By_Age");

            migrationBuilder.DropTable(
                name: "Ons_By_Composition");

            migrationBuilder.DropTable(
                name: "Ons_By_Region");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Subcategories");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
