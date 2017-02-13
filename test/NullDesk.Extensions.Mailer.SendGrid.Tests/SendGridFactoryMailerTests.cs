using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests
{
    public class SendGridFactoryMailerTests : IClassFixture<FactoryMailFixture>
    {

        private FactoryMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        public SendGridFactoryMailerTests(FactoryMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task SendMailWithTemplate(string template, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, a))).ToArray();

            var mailer = Fixture.Mail.StandardMailer;
            mailer.GetType().GetTypeInfo().IsSubclassOf(typeof(SendGridMailer)).Should().BeTrue();

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

            var m = result.Should().NotBeNull().And.BeOfType<MessageDeliveryItem>().Which;
            m.IsSuccess.Should().BeTrue();
            m.MessageData.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendMail(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, a))).ToArray();

            var mailer = Fixture.Mail.SimpleMailer;
            mailer.GetType().GetTypeInfo().IsSubclassOf(typeof(SendGridSimpleMailer)).Should().BeTrue();
            mailer.GetType().GetTypeInfo().IsSubclassOf(typeof(SendGridMailer)).Should().BeFalse();
           
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

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task SendMailWithSurrogateSimpleMailer(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, a))).ToArray();

            //this mailer should only have one registered mailer, and it's a template mailer
            var mailer = Fixture.TemplateMail.SimpleMailer;

            //check that we got the fallback template mailer anyway
            mailer.GetType().GetTypeInfo().IsSubclassOf(typeof(SendGridSimpleMailer)).Should().BeTrue();
            mailer.GetType().GetTypeInfo().IsSubclassOf(typeof(SendGridMailer)).Should().BeTrue();

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