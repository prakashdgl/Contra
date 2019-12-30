using Microsoft.EntityFrameworkCore.Migrations;

namespace Contra.Data.Migrations
{
    public partial class ExtendUserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId1",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId",
                table: "Article",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId1",
                table: "Article",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId2",
                table: "Article",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OpenTalonUserId",
                table: "Comment",
                column: "OpenTalonUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OpenTalonUserId1",
                table: "Comment",
                column: "OpenTalonUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Article_OpenTalonUserId",
                table: "Article",
                column: "OpenTalonUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_OpenTalonUserId1",
                table: "Article",
                column: "OpenTalonUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Article_OpenTalonUserId2",
                table: "Article",
                column: "OpenTalonUserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_AspNetUsers_OpenTalonUserId",
                table: "Article",
                column: "OpenTalonUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Article_AspNetUsers_OpenTalonUserId1",
                table: "Article",
                column: "OpenTalonUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Article_AspNetUsers_OpenTalonUserId2",
                table: "Article",
                column: "OpenTalonUserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_OpenTalonUserId",
                table: "Comment",
                column: "OpenTalonUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_OpenTalonUserId1",
                table: "Comment",
                column: "OpenTalonUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_AspNetUsers_OpenTalonUserId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Article_AspNetUsers_OpenTalonUserId1",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Article_AspNetUsers_OpenTalonUserId2",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_OpenTalonUserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_OpenTalonUserId1",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_OpenTalonUserId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_OpenTalonUserId1",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Article_OpenTalonUserId",
                table: "Article");

            migrationBuilder.DropIndex(
                name: "IX_Article_OpenTalonUserId1",
                table: "Article");

            migrationBuilder.DropIndex(
                name: "IX_Article_OpenTalonUserId2",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "OpenTalonUserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "OpenTalonUserId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OpenTalonUserId",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "OpenTalonUserId1",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "OpenTalonUserId2",
                table: "Article");
        }
    }
}
