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
    public class MailKitSmtpFileTemplateMailerTests : IClassFixture<TemplateMailFixture>
    {

        private TemplateMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        public MailKitSmtpFileTemplateMailerTests(TemplateMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task SendMailWithTemplate(string templateName, string[] attachments)
        {

            var mailer = Fixture.ServiceProvider.GetService<ITemplateMailer>();

            var result =
                await
                    mailer.SendMailAsync(
                        templateName,
                        "noone@toast.com",
                        "No One Important",
                        $"xunit Test run: {templateName}",
                        ReplacementVars,
                        attachments,
                        CancellationToken.None);

            result.Should().BeTrue();
        }
    }
}