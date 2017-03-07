using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
    public partial class v300 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MessageData",
                table: "MessageHistory",
                newName: "SubstitutionsJson");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "MessageHistory",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentsJson",
                table: "MessageHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyJson",
                table: "MessageHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromDisplayName",
                table: "MessageHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromEmailAddress",
                table: "MessageHistory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentsJson",
                table: "MessageHistory");

            migrationBuilder.DropColumn(
                name: "BodyJson",
                table: "MessageHistory");

            migrationBuilder.DropColumn(
                name: "FromDisplayName",
                table: "MessageHistory");

            migrationBuilder.DropColumn(
                name: "FromEmailAddress",
                table: "MessageHistory");

            migrationBuilder.RenameColumn(
                name: "SubstitutionsJson",
                table: "MessageHistory",
                newName: "MessageData");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "MessageHistory",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
