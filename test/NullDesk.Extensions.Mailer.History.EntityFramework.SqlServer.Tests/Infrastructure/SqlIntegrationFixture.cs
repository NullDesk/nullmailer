using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NullDesk.Extensions.Mailer.Core;
using System;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests.Infrastructure
{
    public class SqlIntegrationFixture : IDisposable
    {
        public SqlIntegrationFixture()
        {
            //setup the dependency injection service
            var services = new ServiceCollection();
            services.AddLogging(config => config.AddDebug().SetMinimumLevel(LogLevel.Debug));
            services.AddOptions();
            services.Configure<NullMailerSettings>(s =>
            {
                s.FromDisplayName = "xunit";
                s.FromEmailAddress = "xunit@nowhere.com";
            });


            services.AddMailerHistory<TestSqlHistoryContext>(new SqlEntityHistoryStoreSettings
            {
                SourceApplicationName = "xunit",
                ConnectionString =
                    @"Server=(localdb)\MSSQLLocalDB;Database=NullDeskMailerHistoryTests;Trusted_Connection=True;"
            });

            services.AddNullMailer(s => s.GetService<IOptions<NullMailerSettings>>().Value);

            ServiceProvider = services.BuildServiceProvider();

        }

        public IServiceProvider ServiceProvider { get; set; }


        public void Dispose()
        {
            using (var context =
                ((EntityHistoryStore<TestSqlHistoryContext>)ServiceProvider.GetService<IHistoryStore>())
                .GetHistoryContext())
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}