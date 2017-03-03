using System.Collections.Generic;
using FluentAssertions;
using NullDesk.Extensions.Mailer.Core.Fluent.Extensions;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class NullMailerTests
    {
        private NullMailer GetMailer()
        {
            return new NullMailer(_mailerSettings);
        }

        private readonly NullMailerSettings _mailerSettings = new NullMailerSettings()
        {
            FromEmailAddress = "nowhere@nowhere.com",
            FromDisplayName = "Mr. NoBody"
        };


        [Fact]
        public void Create_FromIBuilderStepsCompleted()
        {

            var mailer = GetMailer();
            mailer.CreateMessage(b => b
                .Subject("Some Topic")
                .And.To("nooneelse@nowhere.com")
                .And.ForBody()
                .WithPlainText("body text"));

            ((IMailer)mailer).Deliverables.Should().Contain(m => m.Message.Subject == "Some Topic");
        }

        [Fact]
        public void Create_MailerMessage()
        {
            var mailer = GetMailer();
            mailer.CreateMessage(b => b
                .Subject("Some Topic")
                .And.To("nooneelse@nowhere.com")
                .And.ForBody()
                .WithPlainText("body text")
                .Build());

            ((IMailer)mailer).Deliverables
                 .Should().NotBeEmpty()
                 .And.Contain(m => m.Message.Subject == "Some Topic");
        }

        [Fact]
        public void AddMessage()
        {
            var mailer = GetMailer();
            mailer.AddMessage(new MailerMessage()
                .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                .WithSubject("Some Topic"));

            ((IMailer)mailer).Deliverables
                 .Should().NotBeEmpty()
                 .And.Contain(m => m.Message.Subject == "Some Topic");
        }

        [Fact]
        public void AddMessages()
        {
            var mailer = GetMailer();
            mailer.AddMessages(new List<MailerMessage>
            {
                new MailerMessage()
                    .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                    .WithSubject("Some Topic"),
                new MailerMessage()
                    .From(_mailerSettings.FromEmailAddress, _mailerSettings.FromDisplayName)
                    .WithSubject("Some Other Topic"),

            });

            ((IMailer)mailer).Deliverables
                .Should().HaveCount(2)
                .And.Contain(m => m.Message.Subject == "Some Topic")
                .And.Contain(m => m.Message.Subject == "Some Other Topic");
        }
    }
}
