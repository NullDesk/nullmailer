using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{
    public class FactoryMailFixture : IDisposable
    {
        public FactoryMailFixture()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Debug);

            var sendGridSettings = new SendGridMailerSettings
            {
                ApiKey = "abc",
                FromDisplayName = "xunit",
                FromEmailAddress = "xunit@nowhere.com"
            };

            var logger = loggerFactory.CreateLogger<SendGridMailer>();

            Mail.Register(() => new SendGridMailerFake(
                new OptionsWrapper<SendGridMailerSettings>(sendGridSettings),
                logger,
                Store));
        }

        public MailerFactory Mail { get; } = new MailerFactory();


        public IHistoryStore Store { get; set; } = new InMemoryHistoryStore();


        public void Dispose()
        {
        }
    }
}