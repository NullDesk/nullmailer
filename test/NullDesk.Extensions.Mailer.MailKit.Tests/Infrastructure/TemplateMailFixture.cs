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
    public class TemplateMailFixture : MailFixture, IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public TemplateMailFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();

            services.AddOptions();

            var templateOptions = new OptionsWrapper<FileTemplateMailerSettings>(
                new FileTemplateMailerSettings
                {
                    TemplatePath = "../TestData/templates"
                });

            var isMailServerAlive = false;
            var lazy = new Lazy<OptionsWrapper<SmtpMailerSettings>>(() => SetupMailerOptions(out isMailServerAlive));

            services.AddTransient<ITemplateMailer>(s =>
            {
                var options = lazy.Value;
                var client = Substitute.For<SmtpClient>();
                client
                    .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                    .Returns(Task.CompletedTask);
                return (isMailServerAlive)
                    ? new MkSmtpFileTemplateMailer(options, templateOptions)
                    : new MkSmtpFileTemplateMailer(client, options, templateOptions);
            });


            ServiceProvider = services.BuildServiceProvider();

        }
        public void Dispose() { }
    }
}