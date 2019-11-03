using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenTalon.Data.Migrations
{
    public partial class SupportTickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerID = table.Column<string>(nullable: true),
                    AssignedTo = table.Column<string>(nullable: true),
                    Approved = table.Column<int>(nullable: false),
                    AuthorName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ticket");
        }
    }
}
