using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTalon.Data.Migrations
{
    public partial class ArticleOwnerID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Article");

            migrationBuilder.AlterColumn<int>(
                name: "Approved",
                table: "Comment",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Approved",
                table: "Comment",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Article",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
