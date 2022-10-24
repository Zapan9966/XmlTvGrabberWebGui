using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace XmlTvGrabberWebGui.Migrations
{
    public partial class TraceFileProcessingId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileProcessingId",
                table: "Traces",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Traces");

            migrationBuilder.CreateTable(
                name: "Traces",
                columns: table => new
                {
                    TraceId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(nullable: true),
                    LogLevel = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    Filename = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traces", x => x.TraceId);
                });
        }
    }
}
