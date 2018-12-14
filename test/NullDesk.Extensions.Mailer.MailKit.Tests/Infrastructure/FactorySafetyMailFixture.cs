using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{
    public class FactorySafetyMailFixture : MailFixture, IDisposable
    {
        public FactorySafetyMailFixture()
        {
            var loggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() }, new LoggerFilterOptions() { MinLevel = LogLevel.Debug });


            Mail = new MailerFactory(loggerFactory, Store);
            var mkSettings = SetupMailerOptions(out var isMailServerAlive).Value;

            if (isMailServerAlive)
            {
                Mail.AddSafetyMailer(new SafetyMailerSettings
                    {
                        SafeRecipientEmailAddress = "safe@nowhere.com"
                    },
                    mkSettings);
            }
            else
            {
                SmtpClient GetClientFunc()
                {
                    var c = Substitute.For<SmtpClient>();
                    c.SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                        .Returns(Task.CompletedTask);
                    return c;
                }

                Mail.AddSafetyMailer(new SafetyMailerSettings
                    {
                        SafeRecipientEmailAddress = "safe@nowhere.com"
                    },
                    GetClientFunc,
                    mkSettings);
            }
        }

        public MailerFactory Mail { get; set; }

        public IHistoryStore Store { get; set; } = new InMemoryHistoryStore();


        public void Dispose()
        {
        }
    }
}