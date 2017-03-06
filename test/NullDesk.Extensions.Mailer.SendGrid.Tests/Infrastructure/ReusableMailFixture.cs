﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{
    public class ReusableMailFixture : IDisposable
    {
        public ReusableMailFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();
            services.AddSingleton<IHistoryStore, InMemoryHistoryStore>();

            services.Configure<SendGridMailerSettings>(s =>
                {
                    s.ApiKey = "abc";
                    s.FromDisplayName = "xunit";
                    s.FromEmailAddress = "xunit@nowhere.com";
                }
            );
            services.AddSingleton<SendGridMailerFake>();

            services.AddSingleton<IMailer>(s => s.GetService<SendGridMailerFake>());


            ServiceProvider = services.BuildServiceProvider();
            var logging = ServiceProvider.GetService<ILoggerFactory>();
            logging.AddDebug(LogLevel.Debug);
        }

        public IServiceProvider ServiceProvider { get; set; }


        public void Dispose()
        {
        }
    }
}