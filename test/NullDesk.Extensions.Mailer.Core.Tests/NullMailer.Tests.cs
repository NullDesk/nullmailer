using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class NullMailerTests
    {
        private NullMailer GetMailer()
        {
            return new NullMailer(_mailerSettings);
        }

        private readonly NullMailerSettings _mailerSettings =
            new NullMailerSettings
            {
                FromEmailAddress = "nowhere@nowhere.com",
                FromDisplayName = "Mr. NoBody"
            };

        [Theory]
        [InlineData(null, "toast@toast.com", "something")]
        [InlineData("toast@toast.com", null, "something")]
        [InlineData("toast@toast.com", "toast@toast.com", null)]
        [Trait("TestType", "Unit")]
        public void NullMailer_AddMessage_FailsWhenNotDeliverable(string from, string to, string textBody)
        {
            var mailer = GetMailer();
            mailer.Invoking(m => m.AddMessage(
                    new MailerMessage
                    {
                        From = string.IsNullOrEmpty(from) ? null : new MessageSender { EmailAddress = from },
                        Recipients = string.IsNullOrEmpty(to)
                            ? new List<MessageRecipient>()
                            : new List<MessageRecipient>
                            {
                                new MessageRecipient
                                {
                                    EmailAddress = to
                                }
                            },
                        Body = string.IsNullOrEmpty(textBody) ? null : new ContentBody().WithPlainText(textBody)
                    })).Should().Throw<ArgumentException>();
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void NullMailer_AddMessage()
        {
            var mailer = GetMailer();
            mailer.AddMessage(new MailerMessage()
                .To("noone@nowhere.com")
                .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                .WithSubject("Some Topic")
                .WithBody<ContentBody>(b => b.PlainTextContent = "something"));

            ((IMailer)mailer).PendingDeliverables
                .Should()
                .NotBeEmpty()
                .And.Contain(m => m.Subject == "Some Topic");
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void NullMailer_AddMessages()
        {
            var mailer = GetMailer();
            mailer.AddMessages(new List<MailerMessage>
            {
                new MailerMessage()
                    .To("noone@nowhere.com")
                    .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                    .WithSubject("Some Topic")
                    .WithBody<ContentBody>(b => b.PlainTextContent = "something"),
                new MailerMessage()
                    .To("noone@nowhere.com")
                    .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                    .WithSubject("Some Other Topic")
                    .WithBody<ContentBody>(b => b.PlainTextContent = "something")
            });

            ((IMailer)mailer).PendingDeliverables
                .Should()
                .HaveCount(2)
                .And.Contain(m => m.Subject == "Some Topic")
                .And.Contain(m => m.Subject == "Some Other Topic");
        }


        [Fact]
        [Trait("TestType", "Unit")]
        public void NullMailer_Create_FromIBuilderStepsCompleted()
        {
            var mailer = GetMailer();
            mailer.CreateMessage(b => b
                .Subject("Some Topic")
                .And.To("nooneelse@nowhere.com")
                .And.ForBody()
                .WithPlainText("body text"));

            ((IMailer)mailer).PendingDeliverables.Should().Contain(m => m.Subject == "Some Topic");
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void NullMailer_Create_MailerMessage()
        {
            var mailer = GetMailer();
            mailer.CreateMessage(b => b
                .Subject("Some Topic")
                .And.To("nooneelse@nowhere.com")
                .And.ForBody()
                .WithPlainText("body text")
                .Build());

            ((IMailer)mailer).PendingDeliverables
                .Should()
                .NotBeEmpty()
                .And.Contain(m => m.Subject == "Some Topic");
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public async Task NullMailer_SafetyMailerReplacesAddress()
        {
            var mailer = new SafetyMailer<NullMailer>(GetMailer(), new SafetyMailerSettings()
            {
                SafeRecipientEmailAddress = "safe@nowhere.com"
            });
            mailer.AddMessage(new MailerMessage()
                .To("noone@nowhere.com")
                .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                .WithSubject("Some Topic")
                .WithBody<ContentBody>(b => b.PlainTextContent = "something"));
            var results = await mailer.SendAllAsync();
            var recipient = results
                .Should()
                .HaveCount(1)
                .And.Subject.First();
            recipient.ToDisplayName
                .Should().NotBeNullOrWhiteSpace()
                .And.Be("(safe) noone@nowhere.com <'noone@nowhere.com'>");
            recipient.ToEmailAddress
                .Should().NotBeNullOrWhiteSpace()
                .And.Be("safe@nowhere.com");
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public async Task NullMailer_SafetyMailerReplacesDisplayName()
        {
            var mailer = new SafetyMailer<NullMailer>(GetMailer(), new SafetyMailerSettings()
            {
                SafeRecipientEmailAddress = "safe@nowhere.com"
            });
            mailer.AddMessage(new MailerMessage()
                .To("noone@nowhere.com", "Friendly Name")
                .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                .WithSubject("Some Topic")
                .WithBody<ContentBody>(b => b.PlainTextContent = "something"));
            var results = await mailer.SendAllAsync();
            var recipient = results
                .Should()
                .HaveCount(1)
                .And.Subject.First();
            recipient.ToDisplayName
                .Should().NotBeNullOrWhiteSpace()
                .And.Be("(safe) Friendly Name <'noone@nowhere.com'>");
            recipient.ToEmailAddress
                .Should().NotBeNullOrWhiteSpace()
                .And.Be("safe@nowhere.com");
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public async Task NullMailer_SafetyMailerReplacesDisplayNameWhenPrependEmpty()
        {
            var mailer = new SafetyMailer<NullMailer>(GetMailer(), new SafetyMailerSettings()
            {
                PrependDisplayNameWithText = string.Empty,
                SafeRecipientEmailAddress = "safe@nowhere.com"
            });
            mailer.AddMessage(new MailerMessage()
                .To("noone@nowhere.com", "Friendly Name")
                .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                .WithSubject("Some Topic")
                .WithBody<ContentBody>(b => b.PlainTextContent = "something"));
            var results = await mailer.SendAllAsync();
            var recipient = results
                .Should()
                .HaveCount(1)
                .And.Subject.First();
            recipient.ToDisplayName
                .Should().NotBeNullOrWhiteSpace()
                .And.Be("Friendly Name <'noone@nowhere.com'>");
            recipient.ToEmailAddress
                .Should().NotBeNullOrWhiteSpace()
                .And.Be("safe@nowhere.com");
        }

        [Fact]
        [Trait("TestType", "Unit")]
        public void NullMailer_SafetyMailerEmptySafeRecipientThrows()
        {
            Action act = () =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new SafetyMailer<NullMailer>(GetMailer(), new SafetyMailerSettings()
                {
                    SafeRecipientEmailAddress = string.Empty
                });
            };
            act.Should().Throw<ArgumentException>().Which.Message.Should().Be(
                "Safety mailer cannot enable safe recipients when SafeRecipientEmailAddress is not specified");

        }
    }
}