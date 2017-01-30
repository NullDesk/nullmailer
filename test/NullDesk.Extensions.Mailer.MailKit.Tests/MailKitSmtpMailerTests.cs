using System.Collections.Generic;
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
    public class MkSmtpMailerTests : IClassFixture<StandardMailFixture>
    {

        private StandardMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        public MkSmtpMailerTests(StandardMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task SendMailWithTemplate(string template, string[] attachments)
        {

            var mailer = Fixture.ServiceProvider.GetService<IStandardMailer>();

            var result =
                await
                    mailer.SendMailAsync(
                        template,
                        "noone@toast.com",
                        "No One Important",
                        $"xunit Test run: {template}",
                        ReplacementVars,
                        attachments,
                        CancellationToken.None);

            result.Should().BeOfType<MessageDeliveryItem>().Which.IsSuccess.Should().BeTrue();
        }
    }
}