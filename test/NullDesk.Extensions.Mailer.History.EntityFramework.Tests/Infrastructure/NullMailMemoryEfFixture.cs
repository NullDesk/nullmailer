using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.Tests.Infrastructure
{
    public class NullMailMemoryEfFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        public NullMailMemoryEfFixture()
        {

            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddOptions();

            var builder = new DbContextOptionsBuilder<TestHistoryContext>().UseInMemoryDatabase("TestHistoryDb");
            services.AddSingleton<DbContextOptions>(s => builder.Options);
            services.AddSingleton<IHistoryStore, EntityHistoryStore<TestHistoryContext>>();
            services.AddTransient<ISimpleMailer, NullSimpleMailer>();

            ServiceProvider = services.BuildServiceProvider();
            
            var logging = ServiceProvider.GetService<ILoggerFactory>();
            logging.AddDebug(LogLevel.Debug);
        }



        public void Dispose()
        {

        }


    }
}
