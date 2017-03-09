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
    [Migration("20170309223806_v3.0.1")]
    partial class v301
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NullDesk.Extensions.Mailer.Core.DeliveryItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AttachmentsJson");

                    b.Property<string>("BodyJson");

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<string>("DeliveryProvider")
                        .HasMaxLength(50);

                    b.Property<string>("ExceptionMessage")
                        .HasMaxLength(500);

                    b.Property<string>("FromDisplayName");

                    b.Property<string>("FromEmailAddress");

                    b.Property<bool>("IsSuccess");

                    b.Property<string>("ProviderMessageId")
                        .HasMaxLength(100);

                    b.Property<string>("Subject");

                    b.Property<string>("SubstitutionsJson");

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
