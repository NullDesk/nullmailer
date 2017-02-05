using System.Collections.Generic;
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

        private FactoryMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        public MailKitSmtpFactoryMailerTests(FactoryMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }



        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task SendMailWithTemplate(string template, string[] attachments)
        {

            var mailer = Fixture.Mail.StandardMailer;
            mailer.Should().BeOfType<MkSmtpMailer>();
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

            var mailer = Fixture.Mail.SimpleMailer;
            mailer.Should().BeOfType<MkSimpleSmtpMailer>();
            mailer.Should().NotBeOfType<MkSmtpMailer>();
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
            //this mailer should only have one registered mailer, and it's a template mailer
            var mailer = Fixture.TemplateMail.SimpleMailer;

            //check that we got the fallback template mailer anyway
            mailer.Should().BeOfType<MkSmtpMailer>();
            mailer.Should().NotBeOfType<MkSimpleSmtpMailer>();
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