using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using NullLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger;

namespace NullDesk.Extensions.Mailer.Core.Tests
{
    public class MailerFactoryTests
    {
        [Fact]
        public void Factory_Registrations_Minimal()
        {
            var factory = new MailerFactory();
            factory.Register<NullMailer, NullMailerSettings>(
                new NullMailerSettings { ReplyToEmailAddress = "junk@toast.com" }
            );

            var nm = factory.GetMailer().Should().BeOfType<NullMailer>().Which;
            nm.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Logger.Should().BeOfType<NullLogger>();
            nm.HistoryStore.Should().BeOfType<NullHistoryStore>();

        }

        [Fact]
        public void Factory_Registrations_WithDefaultHistoryStore()
        {

            var factory = new MailerFactory
            {
                DefaultHistoryStore = new InMemoryHistoryStore()
            };
            factory.Register<NullMailer, NullMailerSettings>(
                new NullMailerSettings { ReplyToEmailAddress = "junk@toast.com" }
                );

            var nm = factory.GetMailer().Should().BeOfType<NullMailer>().Which;
            nm.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Logger.Should().BeOfType<NullLogger>();
            nm.HistoryStore.Should().BeOfType<InMemoryHistoryStore>();

            
        }

        [Fact]
        public void Factory_Registrations_WithDefaultLoggerFactory()
        {
            var factory = new MailerFactory
            {
                DefaultLoggerFactory = new LoggerFactory()
            };
            factory.Register<NullMailer, NullMailerSettings>(
                new NullMailerSettings { ReplyToEmailAddress = "junk@toast.com" }
            );

            var nm = factory.GetMailer().Should().BeOfType<NullMailer>().Which;
            nm.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Logger.Should().BeAssignableTo<ILogger>().And.NotBeOfType<NullLogger>();
            nm.HistoryStore.Should().BeOfType<NullHistoryStore>();

        }

        [Fact]
        public void Factory_Registrations_ExplicitValues()
        {
            var factory = new MailerFactory();
            factory.Register<NullMailer, NullMailerSettings>(
                new NullMailerSettings { ReplyToEmailAddress = "junk@toast.com" },
                new LoggerFactory().CreateLogger(typeof(NullMailer)),
                new InMemoryHistoryStore()
            );

            var nm = factory.GetMailer().Should().BeOfType<NullMailer>().Which;
            nm.Settings.ReplyToEmailAddress.Should().Be("junk@toast.com");
            nm.Logger.Should().BeAssignableTo<ILogger>().And.NotBeOfType<NullLogger>();
            nm.HistoryStore.Should().BeOfType<InMemoryHistoryStore>();

        }



    }
}
