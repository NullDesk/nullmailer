using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests.Migrations
{
    public partial class ReplyTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReplyToDisplayName",
                table: "MessageHistory",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplyToEmailAddress",
                table: "MessageHistory",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyToDisplayName",
                table: "MessageHistory");

            migrationBuilder.DropColumn(
                name: "ReplyToEmailAddress",
                table: "MessageHistory");
        }
    }
}
