using Microsoft.EntityFrameworkCore.Migrations;

namespace XmlTvGrabberWebGui.Migrations
{
    public partial class RenameTvHeadendCategoryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "XmlCategories");

            migrationBuilder.DropTable(
                name: "TvHeadendCategories");

            migrationBuilder.CreateTable(
                name: "TvHeadendCategories",
                columns: table => new
                {
                    TvHeadendCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Group = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvHeadendCategories", x => x.TvHeadendCategoryId);
                });


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
                        principalColumn: "TvHeadendCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_XmlCategories_TvHeadendCategoryId",
                table: "XmlCategories",
                column: "TvHeadendCategoryId");


            //migrationBuilder.DropForeignKey(
            //    name: "FK_XmlCategories_TvHeadendCategories_TvHeadendCategoryId",
            //    table: "XmlCategories");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_TvHeadendCategories",
            //    table: "TvHeadendCategories");

            //migrationBuilder.DropColumn(
            //    name: "TvHeadendCategorieId",
            //    table: "TvHeadendCategories");

            //migrationBuilder.AddColumn<int>(
            //    name: "TvHeadendCategoryId",
            //    table: "TvHeadendCategories",
            //    nullable: false,
            //    defaultValue: 0)
            //    .Annotation("Sqlite:Autoincrement", true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_TvHeadendCategories",
            //    table: "TvHeadendCategories",
            //    column: "TvHeadendCategoryId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_XmlCategories_TvHeadendCategories_TvHeadendCategoryId",
            //    table: "XmlCategories",
            //    column: "TvHeadendCategoryId",
            //    principalTable: "TvHeadendCategories",
            //    principalColumn: "TvHeadendCategoryId",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "XmlCategories");

            migrationBuilder.DropTable(
                name: "TvHeadendCategories");

            migrationBuilder.CreateTable(
                name: "TvHeadendCategories",
                columns: table => new
                {
                    TvHeadendCategorieId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Group = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvHeadendCategories", x => x.TvHeadendCategorieId);
                });

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
    }
}
