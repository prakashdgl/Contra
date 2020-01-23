using Microsoft.EntityFrameworkCore.Migrations;

namespace Contra.Data.Migrations
{
    public partial class ArticleResponseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponseId",
                table: "Article",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseId",
                table: "Article");
        }
    }
}
