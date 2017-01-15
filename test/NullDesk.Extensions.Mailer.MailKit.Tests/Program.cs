using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit.Tests
{
    public class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        static Program()
        {

            //setup the dependency injection service
            var services = new ServiceCollection();

            services.AddOptions();

            var templateOptions = new OptionsWrapper<MailerFileTemplateSettings>(
                new MailerFileTemplateSettings
                {
                    TemplatePath = "./app_data/templates"
                });



            bool isMailServerAlive = false;
            var lazy = new Lazy<OptionsWrapper<SmtpMailerSettings>>(() => SetupMailerOptions(out isMailServerAlive));
            //var options = SetupMailerOptions(out isMailServerAlive);
            services.AddTransient(s =>
            {
                var options = lazy.Value;
                var client = Substitute.For<SmtpClient>();
                client
                  .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                  .Returns(Task.CompletedTask);
                return (isMailServerAlive)
                    ? new MailKitSmtpMailer(options)
                    : new MailKitSmtpMailer(client, options);
            });

            services.AddTransient(s =>
            {
                var options = lazy.Value;
                var client = Substitute.For<SmtpClient>();
                client
                  .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                  .Returns(Task.CompletedTask);
                return (isMailServerAlive)
                    ? new MailKitSmtpFileTemplateMailer(options, templateOptions)
                    : new MailKitSmtpFileTemplateMailer(client, options, templateOptions);
            });


            ServiceProvider = services.BuildServiceProvider();

        }

        private static OptionsWrapper<SmtpMailerSettings> SetupMailerOptions(out bool isMailServerAlive)
        {
            var mailOptions = new OptionsWrapper<SmtpMailerSettings>(
                new SmtpMailerSettings
                {
                    FromEmail = "toast@toast.com",
                    FromDisplayName = "Xunit",
                    SmtpServer = "localhost",
                    SmtpPort = 25,
                    SmtpUseSsl = false
                });

            isMailServerAlive = false;
            TcpClient tcp = new TcpClient();
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

        public static void Main(string[] args)
        {
        }
    }
}
