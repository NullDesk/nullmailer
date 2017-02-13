using System;
using System.IO;
using System.Linq;
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
    public class SendGridSimpleMailerTests : IClassFixture<SimpleMailFixture>
    {
        private SimpleMailFixture Fixture { get; }

        public SendGridSimpleMailerTests(SimpleMailFixture fixture)
        {
            Fixture = fixture;
        }


        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendMail(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, a))).ToArray();

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
            var m = result.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which;
            m.IsSuccess.Should().BeTrue();
            m.MessageData.Should().NotBeNullOrEmpty();
        }


    }
}
