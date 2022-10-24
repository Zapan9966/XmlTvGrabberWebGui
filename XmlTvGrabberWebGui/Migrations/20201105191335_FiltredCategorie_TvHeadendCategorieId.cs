using Microsoft.EntityFrameworkCore.Migrations;

namespace XmlTvGrabberWebGui.Migrations
{
    public partial class FiltredCategorie_TvHeadendCategorieId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiltredCategories");

            migrationBuilder.CreateTable(
                name: "FiltredCategories",
                columns: table => new
                {
                    FiltredCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    TvHeadendCategorieId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltredCategories", x => x.FiltredCategoryId);                    
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FiltredCategories_TvHeadendCategories_TvHeadendCategorieId",
                table: "FiltredCategories",
                column: "TvHeadendCategorieId",
                principalTable: "TvHeadendCategories",
                principalColumn: "TvHeadendCategorieId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiltredCategories");

            migrationBuilder.CreateTable(
                name: "FiltredCategories",
                columns: table => new
                {
                    FiltredCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltredCategories", x => x.FiltredCategoryId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FiltredCategories_TvHeadendCategories_TvHeadendCategorieId",
                table: "FiltredCategories",
                column: "TvHeadendCategorieId",
                principalTable: "TvHeadendCategories",
                principalColumn: "TvHeadendCategorieId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
