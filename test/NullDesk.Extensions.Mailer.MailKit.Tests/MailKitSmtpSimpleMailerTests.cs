using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MkSmtpSimpleMailerTests : IClassFixture<SimpleMailFixture>
    {
        private SimpleMailFixture Fixture { get; }

        public MkSmtpSimpleMailerTests(SimpleMailFixture fixture)
        {
            Fixture = fixture;
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendMail(string html, string text, string[] attachments)
        {
            var mailer = Fixture.ServiceProvider.GetService<ISimpleMailer>();
            var result =
                await
                    mailer.SendMailAsync(
                        "noone@toast.com",
                        "No One Important",
                        "xunit Test run: no template",
                        html,
                        text,
                        attachments,
                        CancellationToken.None
                    );
            result.Should().BeOfType<MessageDeliveryItem>().Which.IsSuccess.Should().BeTrue();
            var m = result.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which;
            m.IsSuccess.Should().BeTrue();
            m.MessageData.Should().NotBeNullOrEmpty();
        }
    }
}