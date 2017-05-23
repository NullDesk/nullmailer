using System;
using Microsoft.Extensions.DependencyInjection;

namespace NullDesk.Extensions.Mailer.Core.Tests.Infrastructure
{
    public class HistorySerializationFixture : IDisposable
    {
        public HistorySerializationFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();

            services.AddOptions();

            services.AddSingleton<IHistoryStore, InMemoryHistoryStore>(
                o => new InMemoryHistoryStore {SerializeAttachments = true});


            ServiceProvider = services.BuildServiceProvider();
        }

        public IServiceProvider ServiceProvider { get; set; }

        public void Dispose()
        {
        }
    }
}