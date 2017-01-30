using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.History.EntityFramework.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;

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

            item.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which.Subject.Should().Be(Subject);
        }
    }
}
