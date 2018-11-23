using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
    public class MkSmtpMailerTests : IClassFixture<StandardMailFixture>
    {
        public MkSmtpMailerTests(StandardMailFixture fixture)
        {
            ReplacementVars.Add("%name%", "Mr. Toast");
            Fixture = fixture;
        }

        private StandardMailFixture Fixture { get; }

        private Dictionary<string, string> ReplacementVars { get; } = new Dictionary<string, string>();

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task MailKit_SendAll_MultipleMessages(string html, string text, string[] attachments)
        {
            var mailer = Fixture.ServiceProvider.GetService<IMailer>();
            var messages = new List<TestParallelMailMessage>();
            for (var i = 0; i < 10; i++)
            {
                messages.Add(new TestParallelMailMessage
                {
                    To = $"noone{i}@toast.com",
                    ToDisplay = "No One Important",
                    Subject = $"#{i} - Multiple Message xunit Test run: no template",
                    Html = html,
                    Text = text
                });
            }
            foreach (var mes in messages)
            {
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
            }
            var result = await mailer.SendAllAsync(CancellationToken.None);

            result
                .Should()
                .NotBeNull()
                .And.AllBeOfType<DeliveryItem>()
                .And.HaveCount(10)
                .And.OnlyContain(i => i.IsSuccess);

            mailer.PendingDeliverables.Should().HaveCount(0);
        }


        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(StandardMailerTestData))]
        public async Task MailKit_SendAll_WithContentBody(string html, string text, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.ServiceProvider.GetService<IMailer>();
            ;


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
                .And.Subject.First()
                .ReplyToEmailAddress.Should()
                .Be("xunitreply@nowhere.com");
        }

        [Theory]
        [Trait("TestType", "Unit")]
        [ClassData(typeof(TemplateMailerTestData))]
        public async Task MailKit_SendAll_WithTemplate(string template, string[] attachments)
        {
            attachments = attachments?.Select(a => Path.Combine(AppContext.BaseDirectory, a)).ToArray();

            var mailer = Fixture.ServiceProvider.GetService<IMailer>();

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
                .And.OnlyContain(i => i.IsSuccess);
        }
    }
}
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
