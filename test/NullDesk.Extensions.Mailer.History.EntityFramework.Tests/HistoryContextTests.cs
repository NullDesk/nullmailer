using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.History.EntityFramework.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;

// ReSharper disable PossibleMultipleEnumeration
namespace NullDesk.Extensions.Mailer.History.EntityFramework.Tests
{
    public class HistoryContextTests : IClassFixture<MemoryEfFixture>
    {
        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        public HistoryContextTests(MemoryEfFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        private const string Subject = "xunit Test run: no template - history";

        private MemoryEfFixture Fixture { get; }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task Ef_History_SendMail(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.ServiceProvider.GetService<IMailer>();
            var deliveryItems =
                mailer.CreateMessage(b => b
                    .Subject(Subject)
                    .And.To("noone@toast.com").WithDisplayName("No One Important")
                    .And.ForBody().WithHtml(html).AndPlainText(text)
                    .And.WithSubstitutions(ReplacementVars)
                    .And.WithAttachments(attachments).Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);
            result
                .Should().NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveSameCount(deliveryItems)
                .And.OnlyContain(i => i.IsSuccess);

            var store = Fixture.ServiceProvider.GetService<IHistoryStore>();

            store.Should().BeOfType<EntityHistoryStore<TestHistoryContext>>();

            var item = await store.GetAsync(result.First().Id, CancellationToken.None);

            var m = item
                .Should().NotBeNull()
                .And.BeOfType<DeliveryItem>();

            m.Which.Body.Should().NotBeNull();
            m.Which.Subject.Should().Be(Subject);

        }


        [Fact]
        [Trait("TestType", "Unit")]
        public async Task Ef_History_List()
        {
            var store = Fixture.ServiceProvider.GetService<IHistoryStore>();

            store.Should().BeOfType<EntityHistoryStore<TestHistoryContext>>();


            for (var x = 0; x < 15; x++)
            {
                await store.AddAsync(new DeliveryItem(MailerMessage.Create(), new MessageRecipient())
                {
                    CreatedDate = DateTimeOffset.Now,
                    ToDisplayName = x.ToString(),
                    Subject = x.ToString(),
                    ToEmailAddress = $"{x}@toast.com",
                    DeliveryProvider = "xunit",
                    Id = Guid.NewGuid(),
                    IsSuccess = true,
                    ExceptionMessage = null,
                    Body = new ContentBody() { PlainTextContent = "content" },
                    FromDisplayName = "noone",
                    FromEmailAddress = "noone@nowhere.com"
                });
            }

            var items = await store.GetAsync(0, 10, CancellationToken.None);

            items.Should().HaveCount(10);

            var secondPageitems = await store.GetAsync(10, 5, CancellationToken.None);

            secondPageitems.Should().HaveCount(5).And.NotBeSameAs(items);
        }
    }
}