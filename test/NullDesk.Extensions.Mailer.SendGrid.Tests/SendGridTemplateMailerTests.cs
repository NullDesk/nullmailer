using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests
{
    public class SendGridTemplateMailerTests : IClassFixture<TemplateMailFixture>
    {

        private TemplateMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        public SendGridTemplateMailerTests(TemplateMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task SendMailWithTemplate(string template, string[] attachments)
        {

            var mailer = Fixture.ServiceProvider.GetService<ITemplateMailer>();

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

            result.Should().BeTrue();
        }
    }
}