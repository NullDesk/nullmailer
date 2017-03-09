// ReSharper disable All
#pragma warning disable 1591
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
    public partial class v301 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderMessageId",
                table: "MessageHistory",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderMessageId",
                table: "MessageHistory");
        }
    }
}
