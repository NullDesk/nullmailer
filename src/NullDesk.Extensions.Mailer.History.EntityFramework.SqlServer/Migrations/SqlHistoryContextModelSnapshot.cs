// ReSharper disable All

#pragma warning disable 1591
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
    [DbContext(typeof(SqlHistoryContext))]
    partial class SqlHistoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NullDesk.Extensions.Mailer.Core.MessageDeliveryItem", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<DateTimeOffset>("CreatedDate");

                b.Property<string>("DeliveryProvider")
                    .HasMaxLength(50);

                b.Property<string>("ExceptionMessage")
                    .HasMaxLength(500);

                b.Property<bool>("IsSuccess");

                b.Property<string>("MessageData");

                b.Property<string>("Subject")
                    .HasMaxLength(250);

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