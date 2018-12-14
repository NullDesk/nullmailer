using System;
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
    public class StandardMailFixture : MailFixture, IDisposable
    {
        public StandardMailFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging(config => config.AddDebug().SetMinimumLevel(LogLevel.Debug));

            services.AddOptions();

            services.AddSingleton<IHistoryStore, InMemoryHistoryStore>();

            var isMailServerAlive = false;
            var lazy = new Lazy<OptionsWrapper<MkSmtpMailerSettings>>(() => SetupMailerOptions(out isMailServerAlive));

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


        public void Dispose()
        {
        }
    }
}