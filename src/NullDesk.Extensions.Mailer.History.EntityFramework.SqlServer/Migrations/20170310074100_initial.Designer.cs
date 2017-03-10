// ReSharper disable All
#pragma warning disable 1591
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
    [DbContext(typeof(SqlHistoryContext))]
    [Migration("20170310074100_initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NullDesk.Extensions.Mailer.History.EntityFramework.EntityHistoryDeliveryItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AttachmentsJson");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<string>("DeliveryProvider")
                        .HasMaxLength(100);

                    b.Property<string>("ExceptionMessage")
                        .HasMaxLength(500);

                    b.Property<string>("FromDisplayName")
                        .HasMaxLength(200);

                    b.Property<string>("FromEmailAddress")
                        .HasMaxLength(200);

                    b.Property<string>("HtmlContent");

                    b.Property<bool>("IsSuccess");

                    b.Property<string>("ProviderMessageId")
                        .HasMaxLength(200);

                    b.Property<string>("Subject")
                        .HasMaxLength(200);

                    b.Property<string>("SubstitutionsJson");

                    b.Property<string>("TemplateName")
                        .HasMaxLength(255);

                    b.Property<string>("TextContent");

                    b.Property<string>("ToDisplayName")
                        .HasMaxLength(200);

                    b.Property<string>("ToEmailAddress")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("MessageHistory");
                });
        }
    }
}
