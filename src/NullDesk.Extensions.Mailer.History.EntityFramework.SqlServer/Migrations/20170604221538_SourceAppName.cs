using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
    public partial class SourceAppName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "SourceApplicationName",
                "MessageHistory",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "SourceApplicationName",
                "MessageHistory");
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member