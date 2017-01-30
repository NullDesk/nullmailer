using System;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{

    public class FactoryMailFixture : IDisposable
    {
        public MailerFactory Mail { get; } = new MailerFactory();

        public MailerFactory TemplateMail { get; } = new MailerFactory();

        public IHistoryStore Store { get; set; } = new InMemoryHistoryStore();


        public FactoryMailFixture()
        {

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Debug);

            var sendGridSettings = new SendGridMailerSettings { ApiKey = "abc" };

            var logger = loggerFactory.CreateLogger<SendGridMailer>();
            var simpleLogger = loggerFactory.CreateLogger<SendGridSimpleMailer>();
            var client = new FakeClient("abc");


            Mail.AddSendGridMailer(client, sendGridSettings, logger, Store);
            Mail.AddSendGridSimpleMailer(client, sendGridSettings, simpleLogger, Store);

            //only has template mailer, but still should be able to complete all tasks for simple mail
            TemplateMail.AddSendGridMailer(client, sendGridSettings, logger, Store);

        }



        public void Dispose()
        {

        }


    }
}
