using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{
    public class ReusableMailFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ReusableMailFixture()
        {

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();
            services.AddSingleton<IHistoryStore, InMemoryHistoryStore>();

            services.Configure<SendGridMailerSettings>(s => s.ApiKey = "abc");

            services.AddSingleton<SendGridSimpleMailerFake>();
                
            services.AddSingleton<ISimpleMailer>(s => s.GetService<SendGridSimpleMailerFake>());


            ServiceProvider = services.BuildServiceProvider();
            var logging = ServiceProvider.GetService<ILoggerFactory>();
            logging.AddDebug(LogLevel.Debug);
        }



        public void Dispose()
        {

        }


    }
}
