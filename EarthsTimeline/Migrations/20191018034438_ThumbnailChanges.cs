using Microsoft.EntityFrameworkCore.Migrations;

namespace EarthsTimeline.Migrations
{
    public partial class ThumbnailChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LargeThumbnailURL",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "SmallThumbnailURL",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailURL",
                table: "Article",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailURL",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "LargeThumbnailURL",
                table: "Article",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmallThumbnailURL",
                table: "Article",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
