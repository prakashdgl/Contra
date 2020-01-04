using Microsoft.EntityFrameworkCore.Migrations;

namespace Contra.Data.Migrations
{
    public partial class UserBio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "OpenTalonUserId",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "OpenTalonUserId1",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "OpenTalonUserId2",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "SensitiveContent",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "SpoilerContent",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "ContraUserId",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContraUserId1",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContraUserId",
                table: "Article",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContraUserId1",
                table: "Article",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContraUserId2",
                table: "Article",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ContraUserId",
                table: "Comment",
                column: "ContraUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ContraUserId1",
                table: "Comment",
                column: "ContraUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Article_ContraUserId",
                table: "Article",
                column: "ContraUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_ContraUserId1",
                table: "Article",
                column: "ContraUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Article_ContraUserId2",
                table: "Article",
                column: "ContraUserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_AspNetUsers_ContraUserId",
                table: "Article",
                column: "ContraUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Article_AspNetUsers_ContraUserId1",
                table: "Article",
                column: "ContraUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Article_AspNetUsers_ContraUserId2",
                table: "Article",
                column: "ContraUserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_ContraUserId",
                table: "Comment",
                column: "ContraUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_ContraUserId1",
                table: "Comment",
                column: "ContraUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_AspNetUsers_ContraUserId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Article_AspNetUsers_ContraUserId1",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Article_AspNetUsers_ContraUserId2",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_ContraUserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_ContraUserId1",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ContraUserId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ContraUserId1",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Article_ContraUserId",
                table: "Article");

            migrationBuilder.DropIndex(
                name: "IX_Article_ContraUserId1",
                table: "Article");

            migrationBuilder.DropIndex(
                name: "IX_Article_ContraUserId2",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "ContraUserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ContraUserId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ContraUserId",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "ContraUserId1",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "ContraUserId2",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId1",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId",
                table: "Article",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId1",
                table: "Article",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenTalonUserId2",
                table: "Article",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SensitiveContent",
                table: "Article",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpoilerContent",
                table: "Article",
                type: "nvarchar(max)",
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
    }
}
