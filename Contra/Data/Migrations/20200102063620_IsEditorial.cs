using Microsoft.EntityFrameworkCore.Migrations;

namespace Contra.Data.Migrations
{
    public partial class IsEditorial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEditorial",
                table: "Article",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEditorial",
                table: "Article");
        }
    }
}
