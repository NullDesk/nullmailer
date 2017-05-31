using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Migrations
{
    [DbContext(typeof(SqlHistoryContext))]
    internal class SqlHistoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
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

                b.Property<string>("ReplyToDisplayName")
                    .HasMaxLength(200);

                b.Property<string>("ReplyToEmailAddress")
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