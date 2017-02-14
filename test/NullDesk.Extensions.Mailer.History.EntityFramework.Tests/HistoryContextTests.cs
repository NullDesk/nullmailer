using System;
using System.Threading;
using System.Threading.Tasks;
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
        private const string Subject = "xunit Test run: no template - history";

        private MemoryEfFixture Fixture { get; }

        public HistoryContextTests(MemoryEfFixture fixture)
        {
            Fixture = fixture;
        }


        [Fact]
        [Trait("TestType", "Unit")]
        public async Task HistoryListTest()
        {
            var store = Fixture.ServiceProvider.GetService<IHistoryStore>();

            store.Should().BeOfType<EntityHistoryStore<TestHistoryContext>>();


            for (var x = 0; x < 15; x++)
            {
                await store.AddAsync(new MessageDeliveryItem
                {
                    CreatedDate = DateTimeOffset.Now,
                    ToDisplayName = x.ToString(),
                    Subject = x.ToString(),
                    ToEmailAddress = $"{x}@toast.com",
                    DeliveryProvider = "xunit",
                    Id = Guid.NewGuid(),
                    IsSuccess = true,
                    MessageData = x.ToString(),
                    ExceptionMessage = null
                });
            }

            var items = await store.GetAsync(0, 10, CancellationToken.None);

            items.Should().HaveCount(10);

            var secondPageitems = await store.GetAsync(10, 5, CancellationToken.None);

            secondPageitems.Should().HaveCount(5).And.NotBeSameAs(items);

        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendMailWithHistory(string html, string text, string[] attachments)
        {
            var mailer = Fixture.ServiceProvider.GetService<ISimpleMailer>();
            var result =
                await
                    mailer.SendMailAsync(
                        "noone@toast.com",
                        "No One Important",
                        Subject,
                        html,
                        text,
                        attachments,
                        CancellationToken.None
                    );
            result.Should().BeOfType<MessageDeliveryItem>().Which.IsSuccess.Should().BeTrue();

            var store = Fixture.ServiceProvider.GetService<IHistoryStore>();

            store.Should().BeOfType<EntityHistoryStore<TestHistoryContext>>();

            var item = await store.GetAsync(result.Id, CancellationToken.None);

            var m = item.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which;
            m.Subject.Should().Be(Subject);
            m.MessageData.Should().NotBeNullOrEmpty();
        }
    }
}
