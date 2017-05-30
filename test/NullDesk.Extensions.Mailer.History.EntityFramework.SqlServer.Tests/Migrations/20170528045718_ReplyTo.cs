using Microsoft.EntityFrameworkCore.Migrations;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests.Migrations
{
    public partial class ReplyTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "ReplyToDisplayName",
                "MessageHistory",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ReplyToEmailAddress",
                "MessageHistory",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "ReplyToDisplayName",
                "MessageHistory");

            migrationBuilder.DropColumn(
                "ReplyToEmailAddress",
                "MessageHistory");
        }
    }
}