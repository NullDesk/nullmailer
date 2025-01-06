using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{
    public class HistoryMailFixture : MailFixture, IDisposable
    {
        public HistoryMailFixture()
        {
            var loggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() }, new LoggerFilterOptions() { MinLevel = LogLevel.Debug });


            // ReSharper disable once UnusedVariable
            var mkSettings = SetupMailerOptions(out bool isMailServerAlive).Value;

            SmtpClient GetClientFunc()
            {
                var c = Substitute.For<SmtpClient>();
                c.SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(string.Empty));
                return c;
            }

            var logger = loggerFactory.CreateLogger<MkSmtpMailer>();


            MailerFactoryForHistoryWithSerializableAttachments.AddMkSmtpMailer(GetClientFunc, mkSettings, logger,
                StoreWithSerializableAttachments);

            MailerFactoryForHistoryWithoutSerializableAttachments.AddMkSmtpMailer(GetClientFunc, mkSettings, logger,
                StoreWithoutSerializableAttachments);
        }

        public MailerFactory MailerFactoryForHistoryWithSerializableAttachments { get; } = new MailerFactory();

        public MailerFactory MailerFactoryForHistoryWithoutSerializableAttachments { get; } = new MailerFactory();


        public IHistoryStore StoreWithSerializableAttachments { get; set; } =
            new InMemoryHistoryStore(
                new StandardHistoryStoreSettings { StoreAttachmentContents = true, SourceApplicationName = "xunit" });

        public IHistoryStore StoreWithoutSerializableAttachments { get; set; } =
            new InMemoryHistoryStore(
                new StandardHistoryStoreSettings { StoreAttachmentContents = false, SourceApplicationName = "xunit" });


        public void Dispose()
        {
        }
    }
}