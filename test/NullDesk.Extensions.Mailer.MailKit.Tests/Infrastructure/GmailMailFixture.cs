﻿using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NSubstitute;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.MailKit.Authentication;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.MailKit.Tests.Infrastructure
{
    public class GmailMailFixture : IDisposable
    {
        public GmailMailFixture()
        {
            //setup the dependency injection service
            SetupBasic();
            SetupToken();
        }

        public IServiceProvider BasicAuthServiceProvider { get; set; }

        public IServiceProvider TokenAuthServiceProvider { get; set; }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        private void SetupBasic()
        {
            var services = new ServiceCollection();


            services.AddOptions();

            services.AddSingleton<IHistoryStore, InMemoryHistoryStore>();

            var settings = SetupMailerOptionsBasic();

            services.AddTransient<IMailer>(s =>
            {
                var client = Substitute.For<SmtpClient>();
                client
                    .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                    .Returns(Task.CompletedTask);
                return new MkSmtpMailer(settings.Value, s.GetService<ILogger<MkSmtpMailer>>(),
                    s.GetService<IHistoryStore>());
            });


            BasicAuthServiceProvider = services.BuildServiceProvider();
        }

        private void SetupToken()
        {
            var services = new ServiceCollection();

            services.AddOptions();

            services.AddSingleton<IHistoryStore, InMemoryHistoryStore>();


            var settings = SetupMailerOptionsToken();

            services.AddTransient<IMailer>(s =>
            {
                var client = Substitute.For<SmtpClient>();
                client
                    .SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
                    .Returns(Task.CompletedTask);

                return new MkSmtpMailer(settings.Value, s.GetService<ILogger<MkSmtpMailer>>(),
                    s.GetService<IHistoryStore>());
            });


            TokenAuthServiceProvider = services.BuildServiceProvider();
        }

        protected OptionsWrapper<MkSmtpMailerSettings> SetupMailerOptionsBasic()
        {
            return new OptionsWrapper<MkSmtpMailerSettings>(
                new MkSmtpMailerSettings
                {
                    FromDisplayName = "xunit",
                    FromEmailAddress = "abc@xyz.com",
                    SmtpServer = "smtp.gmail.com",
                    SmtpPort = 465,
                    SmtpRequireSsl = true,
                    AuthenticationSettings = new MkSmtpAuthenticationSettings
                    {
                        Password = "abc",
                        UserName = "abc@xyz.com"
                    },
                    TemplateSettings = new MkFileTemplateSettings
                    {
                        TemplatePath =
                            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\TestData\templates"))
                    }
                });
        }

        protected OptionsWrapper<MkSmtpMailerSettings> SetupMailerOptionsToken()
        {
            var certificate = X509CertificateLoader.LoadCertificateFromFile(@"C:\Users\steph\Downloads\NullMailerTests-ec989cf935fc.p12");
            //,"notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer("nullmailertests@nullmailertests.iam.gserviceaccount.com")
                {
                    // Note: other scopes can be found here: https://developers.google.com/gmail/api/auth/scopes
                    Scopes = new[] { "https://mail.google.com/" },
                    User = "abc@xyz.com"
                }.FromCertificate(certificate));

            var result = credential.RequestAccessTokenAsync(CancellationToken.None).Result;


            return new OptionsWrapper<MkSmtpMailerSettings>(
                new MkSmtpMailerSettings
                {
                    FromDisplayName = "xunit",
                    FromEmailAddress = "abc@xyz.com",
                    SmtpServer = "smtp.gmail.com",
                    SmtpPort = 465,
                    SmtpRequireSsl = true,
                    AuthenticationSettings = new MkSmtpAuthenticationSettings
                    {
                        Authenticator = new MkSmtpAccessTokenAuthenticator
                        {
                            AccessTokenFactory = () => credential.Token.AccessToken,
                            UserName = "abc@xyz.com"
                        }
                    },
                    TemplateSettings = new MkFileTemplateSettings
                    {
                        TemplatePath =
                            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\TestData\templates"))
                    }
                });
        }
    }
}