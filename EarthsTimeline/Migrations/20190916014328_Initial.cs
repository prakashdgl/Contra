using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EarthsTimeline.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Approved = table.Column<bool>(nullable: false),
                    AuthorId = table.Column<int>(nullable: false),
                    AuthorName = table.Column<string>(nullable: true),
                    LargeThumbnailURL = table.Column<string>(nullable: true),
                    SmallThumbnailURL = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    SummaryShort = table.Column<string>(nullable: true),
                    SummaryLong = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Approved = table.Column<bool>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    AuthorId = table.Column<int>(nullable: false),
                    AuthorName = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    ArticleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ArticleId",
                table: "Comment",
                column: "ArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Article");
        }
    }
}
