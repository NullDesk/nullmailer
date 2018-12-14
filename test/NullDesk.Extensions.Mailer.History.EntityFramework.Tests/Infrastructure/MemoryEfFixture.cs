using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.Tests.Infrastructure
{
    public class MemoryEfFixture : IDisposable
    {
        public MemoryEfFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging(config => config.AddDebug().SetMinimumLevel(LogLevel.Debug));

            services.AddLogging();
            services.AddOptions();
            services.Configure<NullMailerSettings>(s =>
            {
                s.FromDisplayName = "xunit";
                s.FromEmailAddress = "xunit@nowhere.com";
            });

            var builder = new DbContextOptionsBuilder<HistoryContext>()
                .UseInMemoryDatabase("TestHistoryDb");

            services.AddMailerHistory<TestHistoryContext>(
                s => new EntityHistoryStoreSettings {DbOptions = builder.Options});
            services.AddNullMailer(s => s.GetService<IOptions<NullMailerSettings>>().Value);

            ServiceProvider = services.BuildServiceProvider();


        }

        public IServiceProvider ServiceProvider { get; set; }


        public void Dispose()
        {
        }
    }
}