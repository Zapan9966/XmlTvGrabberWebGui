using Microsoft.EntityFrameworkCore.Migrations;

namespace XmlTvGrabberWebGui.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    ConfigId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    XmlUrl = table.Column<string>(nullable: false),
                    OutputFilename = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.ConfigId);
                });

            migrationBuilder.CreateTable(
                name: "TvHeadendCategories",
                columns: table => new
                {
                    TvHeadendCategorieId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvHeadendCategories", x => x.TvHeadendCategorieId);
                });

            migrationBuilder.CreateTable(
                name: "FiltredCategories",
                columns: table => new
                {
                    FiltredCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    TvHeadendCategorieId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltredCategories", x => x.FiltredCategoryId);
                    table.ForeignKey(
                        name: "FK_FiltredCategories_TvHeadendCategories_TvHeadendCategorieId",
                        column: x => x.TvHeadendCategorieId,
                        principalTable: "TvHeadendCategories",
                        principalColumn: "TvHeadendCategorieId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FiltredCategories_TvHeadendCategorieId",
                table: "FiltredCategories",
                column: "TvHeadendCategorieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "FiltredCategories");

            migrationBuilder.DropTable(
                name: "TvHeadendCategories");
        }
    }
}
