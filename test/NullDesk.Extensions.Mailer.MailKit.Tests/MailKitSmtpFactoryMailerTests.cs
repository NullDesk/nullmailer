using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MailKitSmtpFactoryMailerTests : IClassFixture<FactoryMailFixture>
    {
        public MailKitSmtpFactoryMailerTests(FactoryMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        private FactoryMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();


        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task SendAll_WithTemplate(string template, string[] attachments)
        {
            attachments =
                attachments?.Select(a => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, a))).ToArray();

            var mailer = Fixture.Mail.Mailer;
            mailer.Should().BeOfType<MkSmtpMailer>();
            var deliveryItems =
                mailer.CreateMessage(b => b
                    .Subject($"xunit Test run: %template%")
                    .And.To("noone@toast.com").WithDisplayName("No One Important")
                    .And.ForTemplate(template)
                    .And.WithSubstitutions(ReplacementVars)
                    .And.WithSubstitution("%template%", template)
                    .And.WithAttachments(attachments).Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);

            result
                .Should().NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveSameCount(deliveryItems)
                .And.OnlyContain(i => i.IsSuccess);
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendAll_WithContentBody(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.Mail.Mailer;
            mailer.Should().BeOfType<MkSmtpMailer>();
            var deliveryItems =
                mailer.CreateMessage(b => b
                    .Subject($"xunit Test run: content body")
                    .And.To("noone@toast.com").WithDisplayName("No One Important")
                    .And.ForBody().WithHtml(html).AndPlainText(text)
                    .And.WithSubstitutions(ReplacementVars)
                    .And.WithAttachments(attachments).Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);

            result
                .Should().NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveSameCount(deliveryItems)
                .And.OnlyContain(i => i.IsSuccess);
        }
    }
}