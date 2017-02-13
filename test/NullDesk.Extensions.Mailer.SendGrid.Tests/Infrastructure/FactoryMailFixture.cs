using System;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;
using Microsoft.Extensions.Options;

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

            Mail.Register(() => new SendGridMailerFake(
               new OptionsWrapper<SendGridMailerSettings>(sendGridSettings),
               logger,
               Store));

            Mail.Register(() => new SendGridSimpleMailerFake(
                new OptionsWrapper<SendGridMailerSettings>(sendGridSettings),
                simpleLogger, 
                Store));


            //only has template mailer, but still should be able to complete all tasks for simple mail
            TemplateMail.Register(() => new SendGridMailerFake(
              new OptionsWrapper<SendGridMailerSettings>(sendGridSettings),
              logger,
              Store));
        }



        public void Dispose()
        {

        }


    }

}
