using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{

    public class FactoryMailFixture : IDisposable
    {
        public MailerFactory Mail { get; set; }

        public MailerFactory TemplateMail { get; set; }

        public FactoryMailFixture()
        {

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Debug);

            var sendGridSettings = new SendGridMailerSettings {ApiKey = "abc"};

            var logger = loggerFactory.CreateLogger<SendGridMailer>();
            var simpleLogger = loggerFactory.CreateLogger<SendGridSimpleMailer>();
            var client = new FakeClient("abc");

            Mail = new MailerFactory();
            Mail.RegisterSendGridMailer(client, sendGridSettings, logger);
            Mail.RegisterSendGridSimpleMailer(client, sendGridSettings, simpleLogger);

            //only has template mailer, but still should be able to complete all tasks for simple mail
            TemplateMail = new MailerFactory();
            TemplateMail.RegisterSendGridMailer(client, sendGridSettings, logger);

        }



        public void Dispose()
        {

        }


    }
}
