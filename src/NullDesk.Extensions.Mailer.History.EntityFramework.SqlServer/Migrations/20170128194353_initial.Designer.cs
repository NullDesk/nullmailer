// ReSharper disable All
#pragma warning disable 1591

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
    [DbContext(typeof(SqlHistoryContext))]
    [Migration("20170128194353_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NullDesk.Extensions.Mailer.History.HistoryItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DeliveryProvider")
                        .HasMaxLength(50);

                    b.Property<bool>("IsSuccess");

                    b.Property<string>("MessageData")
                        .HasMaxLength(2147483647);

                    b.Property<DateTimeOffset>("MessageDate");

                    b.Property<string>("Subject")
                        .HasMaxLength(250);

                    b.Property<string>("ToDisplayName")
                        .HasMaxLength(200);

                    b.Property<string>("ToEmailAddress")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("HistoryItems");
                });
        }
    }
}
