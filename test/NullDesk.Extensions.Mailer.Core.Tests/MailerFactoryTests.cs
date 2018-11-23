using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class MailerFactoryTests
    {
        [Fact]
        public void Factory_Registrations_ExplicitValues()
        {
            var factory = new MailerFactory();
            factory.Register<NullMailer, NullMailerSettings>(
                new NullMailerSettings {ReplyToEmailAddress = "junk@toast.com"},
                new LoggerFactory().CreateLogger(typeof(NullMailer)),
                new InMemoryHistoryStore()
            );

            var nm = factory.GetMailer().Should().BeOfType<NullMailer>().Which;
            nm.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Logger.Should().BeAssignableTo<ILogger>().And.NotBeOfType<NullLogger>();
            nm.HistoryStore.Should().BeOfType<InMemoryHistoryStore>();
        }

        [Fact]
        public void Factory_Registrations_Minimal()
        {
            var factory = new MailerFactory();
            factory.Register<NullMailer, NullMailerSettings>(
                new NullMailerSettings {ReplyToEmailAddress = "junk@toast.com"}
            );

            var nm = factory.GetMailer().Should().BeOfType<NullMailer>().Which;
            nm.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Logger.Should().BeOfType<NullLogger>();
            nm.HistoryStore.Should().BeOfType<NullHistoryStore>();
        }

        [Fact]
        public void Factory_Registrations_WithDefaultHistoryStore()
        {
            var factory = new MailerFactory(null, new InMemoryHistoryStore());
            factory.Register<NullMailer, NullMailerSettings>(
                new NullMailerSettings {ReplyToEmailAddress = "junk@toast.com"}
            );

            var nm = factory.GetMailer().Should().BeOfType<NullMailer>().Which;
            nm.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Logger.Should().BeOfType<NullLogger>();
            nm.HistoryStore.Should().BeOfType<InMemoryHistoryStore>();
        }

        [Fact]
        public void Factory_Registrations_WithDefaultLoggerFactory()
        {
            var factory = new MailerFactory(new LoggerFactory());
            factory.Register<NullMailer, NullMailerSettings>(
                new NullMailerSettings {ReplyToEmailAddress = "junk@toast.com"}
            );

            var nm = factory.GetMailer().Should().BeOfType<NullMailer>().Which;
            nm.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Logger.Should().BeAssignableTo<ILogger>().And.NotBeOfType<NullLogger>();
            nm.HistoryStore.Should().BeOfType<NullHistoryStore>();
        }

        [Fact]
        public void Factory_Registrations_WithSafetyMailerSettings()
        {
            var factory = new MailerFactory(new LoggerFactory());
            factory.RegisterProxy<SafetyMailer<NullMailer>, SafetyMailerSettings, NullMailer, NullMailerSettings>(
                new SafetyMailerSettings() { SafeRecipientEmailAddress = "safe@toast.com"},
                new NullMailerSettings { ReplyToEmailAddress = "junk@toast.com" }
            );

            var nm = factory.GetMailer().Should().BeOfType<SafetyMailer<NullMailer>>().Which;
            nm.Mailer.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Mailer.Logger.Should().BeAssignableTo<ILogger>().And.NotBeOfType<NullLogger>();
            nm.Mailer.HistoryStore.Should().BeOfType<NullHistoryStore>();
        }

        [Fact]
        public void Factory_Registrations_GetMailerFromSafetyMailer()
        {
            var factory = new MailerFactory(new LoggerFactory());
            factory.RegisterProxy<SafetyMailer<NullMailer>, SafetyMailerSettings, NullMailer, NullMailerSettings>(
                new SafetyMailerSettings() { SafeRecipientEmailAddress = "safe@toast.com" },
                new NullMailerSettings { ReplyToEmailAddress = "junk@toast.com" }
            );

            var nmSpecific = factory.GetMailer<NullMailer>().Should().BeOfType<SafetyMailer<NullMailer>>().Which;
            nmSpecific.Mailer.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nmSpecific.Mailer.Logger.Should().BeAssignableTo<ILogger>().And.NotBeOfType<NullLogger>();
            nmSpecific.Mailer.HistoryStore.Should().BeOfType<NullHistoryStore>();
        }
    }
}