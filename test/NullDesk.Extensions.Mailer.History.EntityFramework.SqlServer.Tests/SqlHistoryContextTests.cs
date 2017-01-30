using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests.Infrastructure;
using Xunit;
using NullDesk.Extensions.Mailer.Tests.Common;
using FluentAssertions;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests
{
    public class SqlHistoryContextTests : IClassFixture<SqlIntegrationFixture>
    {
        private const string Subject = "xunit Test run: no template - history";

        private SqlIntegrationFixture Fixture { get; }

        public SqlHistoryContextTests(SqlIntegrationFixture fixture)
        {
            Fixture = fixture;
        }

        [Theory]
        [Trait("TestType", "Integration")]
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

            store.Should().BeOfType<EntityHistoryStore<SqlHistoryContext>>();

            var item = await store.GetAsync(result.Id, CancellationToken.None);

            item.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which.Subject.Should().Be(Subject);
        }
    }
}
