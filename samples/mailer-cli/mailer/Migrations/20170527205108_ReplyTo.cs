using Microsoft.EntityFrameworkCore.Migrations;

namespace Sample.Mailer.Cli.Migrations
{
    public partial class ReplyTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "ReplyToDisplayName",
                schema: "mailerCli",
                table: "MessageHistory",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ReplyToEmailAddress",
                schema: "mailerCli",
                table: "MessageHistory",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "ReplyToDisplayName",
                schema: "mailerCli",
                table: "MessageHistory");

            migrationBuilder.DropColumn(
                "ReplyToEmailAddress",
                schema: "mailerCli",
                table: "MessageHistory");
        }
    }
}