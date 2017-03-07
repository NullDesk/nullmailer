using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.Tests.Infrastructure
{
    public class MemoryEfFixture : IDisposable
    {
        public MemoryEfFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();

            services.Configure<NullMailerSettings>(s =>
            {
                s.FromDisplayName = "xunit";
                s.FromEmailAddress = "xunit@nowhere.com";
            });

            var builder = new DbContextOptionsBuilder<TestHistoryContext>().UseInMemoryDatabase("TestHistoryDb");
            services.AddSingleton<DbContextOptions>(s => builder.Options);
            services.AddSingleton<IHistoryStore, EntityHistoryStore<TestHistoryContext>>();
            services.AddTransient<IMailer, NullMailer>();

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