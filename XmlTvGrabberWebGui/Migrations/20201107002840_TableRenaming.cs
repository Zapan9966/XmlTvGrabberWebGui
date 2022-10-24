using Microsoft.EntityFrameworkCore.Migrations;

namespace XmlTvGrabberWebGui.Migrations
{
    public partial class TableRenaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiltredCategories");

            migrationBuilder.CreateTable(
                name: "XmlCategories",
                columns: table => new
                {
                    XmlCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    TvHeadendCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XmlCategories", x => x.XmlCategoryId);
                    table.ForeignKey(
                        name: "FK_XmlCategories_TvHeadendCategories_TvHeadendCategoryId",
                        column: x => x.TvHeadendCategoryId,
                        principalTable: "TvHeadendCategories",
                        principalColumn: "TvHeadendCategorieId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_XmlCategories_TvHeadendCategoryId",
                table: "XmlCategories",
                column: "TvHeadendCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "XmlCategories");

            migrationBuilder.CreateTable(
                name: "FiltredCategories",
                columns: table => new
                {
                    FiltredCategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TvHeadendCategorieId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltredCategories", x => x.FiltredCategoryId);
                    table.ForeignKey(
                        name: "FK_FiltredCategories_TvHeadendCategories_TvHeadendCategorieId",
                        column: x => x.TvHeadendCategorieId,
                        principalTable: "TvHeadendCategories",
                        principalColumn: "TvHeadendCategorieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FiltredCategories_TvHeadendCategorieId",
                table: "FiltredCategories",
                column: "TvHeadendCategorieId");
        }
    }
}
