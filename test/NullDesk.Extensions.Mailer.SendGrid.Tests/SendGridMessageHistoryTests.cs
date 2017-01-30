using System.Threading;
using System.Threading.Tasks;
using NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests
{
    public class SendGridMessageHistoryTests : IClassFixture<FactoryMailFixture>
    {

        private const string Subject = "xunit Test run: no template - resend";

        private FactoryMailFixture Fixture { get; }

        public SendGridMessageHistoryTests(FactoryMailFixture fixture)
        {
            Fixture = fixture;
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task ReSendMail(string html, string text, string[] attachments)
        {
            var mailer = Fixture.Mail.SimpleMailer;
            mailer.Should().BeOfType<SendGridSimpleMailer>();
            mailer.Should().NotBeOfType<SendGridMailer>();
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
            result.Should().BeOfType<MessageDeliveryItem>();
            var history = await Fixture.Store.GetAsync(result.Id, CancellationToken.None);
            history.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which.Subject.Should().Be(Subject);
        }
    }
}
