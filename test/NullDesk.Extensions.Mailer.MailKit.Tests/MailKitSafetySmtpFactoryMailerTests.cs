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
    public class MailKitSafetySmtpFactoryMailerTests : IClassFixture<FactorySafetyMailFixture>
    {
        public MailKitSafetySmtpFactoryMailerTests(FactorySafetyMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        private FactorySafetyMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();


        /// <summary>
        /// Mails the kit safety factory send all with template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>Task.</returns>
        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task MailKitSafety_Factory_SendAll_WithTemplate(string template, string[] attachments)
        {
            attachments =
                attachments?.Select(a => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, a))).ToArray();

            var mailer = Fixture.Mail.GetMailer();
            mailer.Should().BeOfType<SafetyMailer<MkSmtpMailer>>();
            var deliveryItems =
                mailer.CreateMessage(b => b
                    .Subject($"xunit Test run: %template%")
                    .And.To("noone@toast.com")
                    .WithDisplayName("No One Important")
                    .And.ForTemplate(template)
                    .And.WithSubstitutions(ReplacementVars)
                    .And.WithSubstitution("%template%", template)
                    .And.WithAttachments(attachments)
                    .Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);

            result
                .Should()
                .NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveSameCount(deliveryItems)
                .And.OnlyContain(i => i.IsSuccess)
                .And.OnlyContain(i => i.ToEmailAddress == "safe@nowhere.com")
                .And.OnlyContain(i => i.ToDisplayName == "(safe) No One Important <noone@toast.com>");
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task MailKitSafety_Factory_SendAll_WithContentBody(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.Mail.GetMailer();
            mailer.Should().BeOfType<SafetyMailer<MkSmtpMailer>>();
            var deliveryItems =
                mailer.CreateMessage(b => b
                    .Subject($"xunit Test run: content body")
                    .And.To("noone@toast.com")
                    .WithDisplayName("No One Important")
                    .And.ForBody()
                    .WithHtml(html)
                    .AndPlainText(text)
                    .And.WithSubstitutions(ReplacementVars)
                    .And.WithAttachments(attachments)
                    .Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);

            result
                .Should()
                .NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveSameCount(deliveryItems)
                .And.OnlyContain(i => i.IsSuccess)
                .And.OnlyContain(i => i.ToEmailAddress == "safe@nowhere.com")
                .And.OnlyContain(i => i.ToDisplayName == "(safe) No One Important <noone@toast.com>");
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task MailKitSafety_Factory_DisposeMailer(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.Mail.GetMailer();
            mailer.Should().BeOfType<SafetyMailer<MkSmtpMailer>>();
            var deliveryItems =
                mailer.CreateMessage(b => b
                    .Subject($"xunit Test run: content body")
                    .And.To("noone@toast.com")
                    .WithDisplayName("No One Important")
                    .And.ForBody()
                    .WithHtml(html)
                    .AndPlainText(text)
                    .And.WithSubstitutions(ReplacementVars)
                    .And.WithAttachments(attachments)
                    .Build());

            var result = await mailer.SendAllAsync(CancellationToken.None);

            result
                .Should()
                .NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveSameCount(deliveryItems)
                .And.OnlyContain(i => i.IsSuccess)
                .And.OnlyContain(i => i.ToEmailAddress == "safe@nowhere.com")
                .And.OnlyContain(i => i.ToDisplayName == "(safe) No One Important <noone@toast.com>");

            mailer.Dispose();
        }
    }
}