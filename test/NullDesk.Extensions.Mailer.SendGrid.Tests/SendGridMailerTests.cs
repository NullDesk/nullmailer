using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests
{
    public class SendGridMailerTests : IClassFixture<StandardMailFixture>
    {
        private StandardMailFixture Fixture { get; }

        public SendGridMailerTests(StandardMailFixture fixture)
        {
            Fixture = fixture;
        }


        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendMail(string html, string text, string[] attachments)
        {

            var mailer = Fixture.ServiceProvider.GetService<IMailer>();

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
            result.Should().BeTrue();
        }

       
    }
}
