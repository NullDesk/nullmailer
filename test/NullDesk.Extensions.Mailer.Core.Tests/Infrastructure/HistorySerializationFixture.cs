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

            services.AddSingleton<IHistoryStore, InMemoryHistoryStore>(
                s => new InMemoryHistoryStore(new StandardHistoryStoreSettings {StoreAttachmentContents = true}));

            ServiceProvider = services.BuildServiceProvider();
        }

        public IServiceProvider ServiceProvider { get; set; }

        public void Dispose()
        {
        }
    }
}