using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{

    public class FactoryMailFixture : IDisposable
    {
        public MailerFactory Mail { get; } = new MailerFactory();

        public MailerFactory TemplateMail { get; } = new MailerFactory();

        public FactoryMailFixture()
        {

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Debug);

            var sendGridSettings = new SendGridMailerSettings { ApiKey = "abc" };

            var logger = loggerFactory.CreateLogger<SendGridMailer>();
            var simpleLogger = loggerFactory.CreateLogger<SendGridSimpleMailer>();
            var client = new FakeClient("abc");


            Mail.AddSendGridMailer(client, sendGridSettings, logger);
            Mail.AddSendGridSimpleMailer(client, sendGridSettings, simpleLogger);

            //only has template mailer, but still should be able to complete all tasks for simple mail
            TemplateMail.AddSendGridMailer(client, sendGridSettings, logger);

        }



        public void Dispose()
        {

        }


    }
}
