using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "MessageHistory",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttachmentsJson = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    DeliveryProvider = table.Column<string>(maxLength: 100, nullable: true),
                    ExceptionMessage = table.Column<string>(maxLength: 500, nullable: true),
                    FromDisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    FromEmailAddress = table.Column<string>(maxLength: 200, nullable: true),
                    HtmlContent = table.Column<string>(nullable: true),
                    IsSuccess = table.Column<bool>(nullable: false),
                    ProviderMessageId = table.Column<string>(maxLength: 200, nullable: true),
                    ReplyToDisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    ReplyToEmailAddress = table.Column<string>(maxLength: 200, nullable: true),
                    Subject = table.Column<string>(maxLength: 200, nullable: true),
                    SubstitutionsJson = table.Column<string>(nullable: true),
                    TemplateName = table.Column<string>(maxLength: 255, nullable: true),
                    TextContent = table.Column<string>(nullable: true),
                    ToDisplayName = table.Column<string>(maxLength: 200, nullable: true),
                    ToEmailAddress = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_MessageHistory", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "MessageHistory");
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}