using System.Net.Sockets;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{
    public class MailFixture
    {
        protected OptionsWrapper<SmtpMailerSettings> SetupMailerOptions(out bool isMailServerAlive)
        {
            var mailOptions = new OptionsWrapper<SmtpMailerSettings>(
                new SmtpMailerSettings
                {
                    FromEmailAddress = "toast@toast.com",
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
    }
}