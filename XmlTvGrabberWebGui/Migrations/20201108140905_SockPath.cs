using Microsoft.EntityFrameworkCore.Migrations;

namespace XmlTvGrabberWebGui.Migrations
{
    public partial class SockPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SockPath",
                table: "Configs",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SockPath",
                table: "Configs");
        }
    }
}
