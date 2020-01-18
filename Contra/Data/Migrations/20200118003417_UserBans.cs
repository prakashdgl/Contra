using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Contra.Data.Migrations
{
    public partial class UserBans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_ContraUserId1",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ContraUserId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Base64Content",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ContraUserId1",
                table: "Comment");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "Image",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Base64Content",
                table: "Image",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContraUserId1",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ContraUserId1",
                table: "Comment",
                column: "ContraUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_ContraUserId1",
                table: "Comment",
                column: "ContraUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
