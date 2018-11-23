using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure;
using NullDesk.Extensions.Mailer.Tests.Common;
using Xunit;
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class MailKitSmtpParallelMailerTests : IClassFixture<ReusableMailFixture>
    {
        public MailKitSmtpParallelMailerTests(ReusableMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        private ReusableMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        [Theory]
        [Trait("TestType", "Integration")]
        [ClassData(typeof(StandardMailerTestData))]
        public void MailKit_Parallel_SendMail(string html, string text, string[] attachments)
        {
            var mailer = Fixture.ServiceProvider.GetService<IMailer>();
            var messages = new List<TestParallelMailMessage>();
            for (var i = 0; i < 10; i++)
            {
                messages.Add(new TestParallelMailMessage
                {
                    To = $"noone{i}@toast.com",
                    ToDisplay = "No One Important",
                    Subject = $"#{i} - Parallel xunit Test run: no template",
                    Html = html,
                    Text = text
                });
            }
            Parallel.ForEach(messages, mes =>
            {
                var deliveryIds =
                    mailer.AddMessage(new MailerMessage
                    {
                        From = new MessageSender {EmailAddress = mes.From},
                        Recipients = new List<MessageRecipient>
                        {
                            new MessageRecipient
                            {
                                EmailAddress = mes.To,
                                DisplayName = mes.ToDisplay
                            }
                        },
                        Subject = mes.Subject,
                        Body = new ContentBody
                        {
                            HtmlContent = mes.Html,
                            PlainTextContent = mes.Text
                        },
                        Substitutions = ReplacementVars
                    });
                var result = mailer.SendAsync(deliveryIds.First()).Result;

                result
                    .Should()
                    .BeOfType<DeliveryItem>()
                    .Which.IsSuccess
                    .Should()
                    .BeTrue();
            });
        }
    }
}
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
