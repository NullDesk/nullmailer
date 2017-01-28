// ReSharper disable All
#pragma warning disable 1591
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DeliveryProvider = table.Column<string>(maxLength: 50, nullable: true),
                    ExceptionMessage = table.Column<string>(maxLength: 500, nullable: true),
                    IsSuccess = table.Column<bool>(nullable: false),
                    MessageData = table.Column<string>(maxLength: 2147483647, nullable: true),
                    MessageDate = table.Column<DateTimeOffset>(nullable: false),
                    Subject = table.Column<string>(maxLength: 250, nullable: true),
                    ToDisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    ToEmailAddress = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryItems");
        }
    }
}
