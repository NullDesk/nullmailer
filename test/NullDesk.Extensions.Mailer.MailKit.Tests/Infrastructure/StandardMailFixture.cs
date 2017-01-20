using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{
    public class StandardMailFixture : MailFixture, IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public StandardMailFixture()
        {
            var services = new ServiceCollection();

            services.AddOptions();

            bool isMailServerAlive = false;
            var lazy = new Lazy<OptionsWrapper<SmtpMailerSettings>>(() => SetupMailerOptions(out isMailServerAlive));
            services.AddTransient<IMailer>(s =>
            {
                var options = lazy.Value;
                var client = Substitute.For<SmtpClient>();
                client
                  .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                  .Returns(Task.CompletedTask);
                return (isMailServerAlive)
                    ? new MkSmtpMailer(options)
                    : new MkSmtpMailer(client, options);
            });
            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose() { }
    }
}
