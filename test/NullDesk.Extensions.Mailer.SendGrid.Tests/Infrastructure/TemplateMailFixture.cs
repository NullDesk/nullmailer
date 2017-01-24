using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;
using SendGrid;

namespace NullDesk.Extensions.Mailer.SendGrid.Tests.Infrastructure
{

    public class TemplateMailFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public TemplateMailFixture()
        {

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddOptions();

            services.Configure<SendGridMailerSettings>(s => s.ApiKey = "abc");
            services.AddTransient<Client>(s => new FakeClient("abc"));
            services.AddTransient<SendGridMailer>();
            services.AddTransient<ITemplateMailer>(s => s.GetService<SendGridMailer>());


            ServiceProvider = services.BuildServiceProvider();

            var logging = ServiceProvider.GetService<ILoggerFactory>();
            logging.AddDebug(LogLevel.Debug);
        }



        public void Dispose()
        {

        }


    }
}
