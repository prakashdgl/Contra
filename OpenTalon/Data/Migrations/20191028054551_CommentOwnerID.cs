using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTalon.Data.Migrations
{
    public partial class CommentOwnerID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Comment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
