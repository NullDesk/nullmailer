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
using NullDesk.Extensions.Mailer.Core.History;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{
    public class StandardMailFixture : MailFixture, IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public StandardMailFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddOptions();

            services.AddSingleton<IHistoryStore,MemoryHistoryStore>();

            var isMailServerAlive = false;
            var lazy = new Lazy<OptionsWrapper<MkSmtpMailerSettings>>(() => SetupMailerOptions(out isMailServerAlive));

            services.AddTransient<IStandardMailer>(s =>
            {
                var options = lazy.Value;
                var client = Substitute.For<SmtpClient>();
                client
                    .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                    .Returns(Task.CompletedTask);
                return (isMailServerAlive)
                    ? new MkSmtpMailer(options, s.GetService<ILogger<MkSmtpMailer>>(), s.GetService<IHistoryStore>())
                    : new MkSmtpMailer(client, options, s.GetService<ILogger<MkSmtpMailer>>(), s.GetService<IHistoryStore>());
            });


            ServiceProvider = services.BuildServiceProvider();
            var logging = ServiceProvider.GetService<ILoggerFactory>();
            logging.AddDebug(LogLevel.Debug);
        }
        public void Dispose() { }
    }
}