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
    public class HistoryMailFixture : MailFixture, IDisposable
    {
        public HistoryMailFixture()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Debug);

            // ReSharper disable once UnusedVariable
            var mkSettings = SetupMailerOptions(out bool isMailServerAlive).Value;

            Func<SmtpClient> getClientFunc = () =>
            {
                var c = Substitute.For<SmtpClient>();
                c
                    .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                    .Returns(Task.CompletedTask);
                return c;
            };

            var logger = loggerFactory.CreateLogger<MkSmtpMailer>();


            MailerFactoryForHistoryWithSerializableAttachments.AddMkSmtpMailer(getClientFunc, mkSettings, logger,
                StoreWithSerializableAttachments);

            MailerFactoryForHistoryWithoutSerializableAttachments.AddMkSmtpMailer(getClientFunc, mkSettings, logger,
                StoreWithoutSerializableAttachments);
        }

        public MailerFactory MailerFactoryForHistoryWithSerializableAttachments { get; } = new MailerFactory();

        public MailerFactory MailerFactoryForHistoryWithoutSerializableAttachments { get; } = new MailerFactory();


        public IHistoryStore StoreWithSerializableAttachments { get; set; } =
            new InMemoryHistoryStore(
                new StandardHistoryStoreSettings {StoreAttachmentContents = true, SourceApplicationName = "xunit"});

        public IHistoryStore StoreWithoutSerializableAttachments { get; set; } =
            new InMemoryHistoryStore(
                new StandardHistoryStoreSettings {StoreAttachmentContents = false, SourceApplicationName = "xunit"});


        public void Dispose()
        {
        }
    }
}