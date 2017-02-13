using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{

    public class FactoryMailFixture : MailFixture, IDisposable
    {
        public MailerFactory Mail { get; set; } = new MailerFactory();

        public MailerFactory TemplateMail { get; set; } = new MailerFactory();

        public IHistoryStore Store { get; set; } = new InMemoryHistoryStore();

        public FactoryMailFixture()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Debug);

            var logger = loggerFactory.CreateLogger<MkSmtpMailer>();
            var simpleLogger = loggerFactory.CreateLogger<MkSimpleSmtpMailer>();

            bool isMailServerAlive = false;
            var mkSettings = SetupMailerOptions(out isMailServerAlive).Value;


            if (isMailServerAlive)
            {
                Mail.AddMkSmtpMailer(mkSettings, logger, Store);
                Mail.AddMkSimpleSmtpMailer(mkSettings, simpleLogger, Store);

                TemplateMail.AddMkSmtpMailer(mkSettings, logger, Store);

            }
            else
            {
                Func<SmtpClient> getClientFunc = () =>
                {
                    var c = Substitute.For<SmtpClient>();
                    c
                        .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                        .Returns(Task.CompletedTask);
                    return c;
                };
                Mail.AddMkSmtpMailer(getClientFunc, mkSettings, logger, Store);
                Mail.AddMkSimpleSmtpMailer(getClientFunc, mkSettings, simpleLogger, Store);

                TemplateMail.AddMkSmtpMailer(getClientFunc, mkSettings, logger, Store);

            }
        }



        public void Dispose()
        {

        }


    }
}
