using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{
    public class FailureMailFixture : IDisposable
    {
        public FailureMailFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging(config => config.AddDebug().SetMinimumLevel(LogLevel.Debug));

            services.AddOptions();

            services.AddSingleton<IHistoryStore, InMemoryHistoryStore>();


            var isMailServerAlive = false;
            var lazy = new Lazy<OptionsWrapper<MkSmtpMailerSettings>>(() => SetupMailerOptions(out isMailServerAlive));
            IsMailServiceAlive = isMailServerAlive;
            services.AddTransient<IMailer>(s =>
            {
                var options = lazy.Value;
                var client = Substitute.For<SmtpClient>();
                client
                    .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                    .Returns(Task.CompletedTask);
                return isMailServerAlive
                    ? new MkSmtpMailer(options.Value, s.GetService<ILogger<MkSmtpMailer>>(),
                        s.GetService<IHistoryStore>())
                    : new MkSmtpMailer(client, options.Value, s.GetService<ILogger<MkSmtpMailer>>(),
                        s.GetService<IHistoryStore>());
            });

            ServiceProvider = services.BuildServiceProvider();

        }

        public IServiceProvider ServiceProvider { get; set; }

        public bool IsMailServiceAlive { get; set; }

        public void Dispose()
        {
        }

        protected OptionsWrapper<MkSmtpMailerSettings> SetupMailerOptions(out bool isMailServerAlive)
        {
            var mailOptions = new OptionsWrapper<MkSmtpMailerSettings>(
                new MkSmtpMailerSettings
                {
                    FromDisplayName = "xunit",
                    FromEmailAddress = "xunit@nowhere.com",
                    SmtpServer = "localhost",
                    SmtpPort = 25,
                    SmtpRequireSsl = false,
                    TemplateSettings = new MkFileTemplateSettings
                    {
                        TemplatePath =
                            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\TestData\templates"))
                    }
                });

            isMailServerAlive = false;
            var tcp = new TcpClient();
            try
            {
                tcp.ConnectAsync(mailOptions.Value.SmtpServer, mailOptions.Value.SmtpPort).Wait();
                isMailServerAlive = tcp.Connected;
            }
            catch
            {
                // ignored
            }

            tcp.Dispose();
            return mailOptions;
        }
    }
}