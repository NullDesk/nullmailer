using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests.Infrastructure
{
    public class SqlIntegrationFixture : IDisposable
    {
        public SqlIntegrationFixture()
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

            var builder =
                new DbContextOptionsBuilder<SqlHistoryContext>().UseSqlServer(
                    @"Server=(localdb)\MSSQLLocalDB;Database=NullDeskMailerHistoryTests;Trusted_Connection=True;");
            services.AddSingleton<DbContextOptions>(s => builder.Options);
            services.AddTransient<SqlHistoryContext>();
            services.AddSingleton<IHistoryStore, EntityHistoryStore<SqlHistoryContext>>();
            services.AddTransient<IMailer, NullMailer>();

            ServiceProvider = services.BuildServiceProvider();

            using (var context = ServiceProvider.GetService<SqlHistoryContext>())
            {
                context.Database.Migrate();
            }

            var logging = ServiceProvider.GetService<ILoggerFactory>();
            logging.AddDebug(LogLevel.Debug);
        }

        public IServiceProvider ServiceProvider { get; set; }


        public void Dispose()
        {
            ServiceProvider.GetService<SqlHistoryContext>().Database.EnsureDeleted();
        }
    }
}