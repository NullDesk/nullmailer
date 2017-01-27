using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{

    public class FactoryMailFixture : MailFixture, IDisposable
    {
        public MailerFactory Mail { get; set; } = new MailerFactory();

        public MailerFactory TemplateMail { get; set; } = new MailerFactory();

        public FactoryMailFixture()
        {

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Debug);

            var logger = loggerFactory.CreateLogger<MkSmtpMailer>();
            var simpleLogger = loggerFactory.CreateLogger<MkSimpleSmtpMailer>();

           

            var isMailServerAlive = false;
            var mkSettings = SetupMailerOptions(out isMailServerAlive).Value;

            var client = Substitute.For<SmtpClient>();
            client
                .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);
            if (isMailServerAlive)
            {
                Mail.AddMkSmtpMailer(mkSettings, logger);
                Mail.AddMkSimpleSmtpMailer(mkSettings, simpleLogger);

                TemplateMail.AddMkSmtpMailer(mkSettings, logger);

            }
            else
            {
                Mail.AddMkSmtpMailer(client, mkSettings, logger);
                Mail.AddMkSimpleSmtpMailer(client, mkSettings, simpleLogger);

                TemplateMail.AddMkSmtpMailer(client, mkSettings, logger);

            }
        }



        public void Dispose()
        {

        }


    }
}
