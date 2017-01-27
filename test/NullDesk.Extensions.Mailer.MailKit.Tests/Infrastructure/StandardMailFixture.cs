﻿using System;
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
        public IServiceProvider ServiceProvider { get; set; }

        public StandardMailFixture()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddOptions();

            var isMailServerAlive = false;
            var lazy = new Lazy<OptionsWrapper<MkSmtpMailerSettings>>(() => SetupMailerOptions(out isMailServerAlive));
            services.AddTransient<ISimpleMailer>(s =>
            {
                var options = lazy.Value;
                var client = Substitute.For<SmtpClient>();
                client
                  .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                  .Returns(Task.CompletedTask);
                return (isMailServerAlive)
                    ? new MkSimpleSmtpMailer(options, s.GetService<ILogger<MkSimpleSmtpMailer>>())
                    : new MkSimpleSmtpMailer(client, options, s.GetService<ILogger<MkSimpleSmtpMailer>>());
            });
            
            ServiceProvider = services.BuildServiceProvider();
            
            var logging = ServiceProvider.GetService<ILoggerFactory>();
            logging.AddDebug(LogLevel.Debug);
        }

        public void Dispose() { }
    }
}
