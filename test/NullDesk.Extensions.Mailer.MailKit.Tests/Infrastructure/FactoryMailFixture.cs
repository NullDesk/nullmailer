using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.Core.History;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{

    public class FactoryMailFixture : MailFixture, IDisposable
    {
        public MailerFactory Mail { get; set; } = new MailerFactory();

        public MailerFactory TemplateMail { get; set; } = new MailerFactory();

        public IHistoryStore Store { get; set; } = new MemoryHistoryStore();

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
                Mail.AddMkSmtpMailer(mkSettings, logger, Store);
                Mail.AddMkSimpleSmtpMailer(mkSettings, simpleLogger, Store);

                TemplateMail.AddMkSmtpMailer(mkSettings, logger, Store);

            }
            else
            {
                Mail.AddMkSmtpMailer(client, mkSettings, logger, Store);
                Mail.AddMkSimpleSmtpMailer(client, mkSettings, simpleLogger, Store);

                TemplateMail.AddMkSmtpMailer(client, mkSettings, logger, Store);

            }
        }



        public void Dispose()
        {

        }


    }
}
