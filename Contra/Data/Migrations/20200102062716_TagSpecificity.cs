using Microsoft.EntityFrameworkCore.Migrations;

namespace Contra.Data.Migrations
{
    public partial class TagSpecificity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SummaryShort",
                table: "Article");

            migrationBuilder.AddColumn<bool>(
                name: "Anonymous",
                table: "Article",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ArticleType",
                table: "Article",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Sensitive",
                table: "Article",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SensitiveContent",
                table: "Article",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Spoiler",
                table: "Article",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SpoilerContent",
                table: "Article",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Article",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anonymous",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "ArticleType",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Sensitive",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "SensitiveContent",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Spoiler",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "SpoilerContent",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "SummaryShort",
                table: "Article",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
