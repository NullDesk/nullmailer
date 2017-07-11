
using Microsoft.EntityFrameworkCore.Migrations;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{

    public partial class ResizeExceptionMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExceptionMessage",
                table: "MessageHistory",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExceptionMessage",
                table: "MessageHistory",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member