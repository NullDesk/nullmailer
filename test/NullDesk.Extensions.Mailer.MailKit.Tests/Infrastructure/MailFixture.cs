﻿using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Options;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{
    public class MailFixture
    {
        protected OptionsWrapper<MkSmtpMailerSettings> SetupMailerOptions(out bool isMailServerAlive)
        {
            var mailOptions = new OptionsWrapper<MkSmtpMailerSettings>(
                new MkSmtpMailerSettings
                {
                    FromEmailAddress = "toast@toast.com",
                    FromDisplayName = "Xunit",
                    SmtpServer = "localhost",
                    SmtpPort = 25,
                    SmtpUseSsl = false,
                    TemplateSettings = new MkFileTemplateSettings()
                    {
                        TemplatePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,@"..\..\..\..\TestData\templates"))
                    }
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