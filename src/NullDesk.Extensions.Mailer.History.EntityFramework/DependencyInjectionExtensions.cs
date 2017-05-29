using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.History.EntityFramework;

namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    /// Class DependencyInjectionExtensions.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        ///     Adds a mailer history store of the specified HistoryContext.
        /// </summary>
        /// <typeparam name="TContext">The type of the History DbContext.</typeparam>
        /// <param name="services">The services collection.</param>
        /// <param name="dbContextOptions">The database context options.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory<TContext>(
            this IServiceCollection services,
            DbContextOptions<HistoryContext> dbContextOptions)
        where TContext : HistoryContext
        {
            //services.AddSingleton(s => dbContextOptions);
            services.AddTransient<HistoryContext, TContext>();

            services.AddSingleton(dbContextOptions);
           
            services.AddSingleton<IHistoryStore, EntityHistoryStore<TContext>>();
            return services;
        }

        /// <summary>
        ///      Adds a mailer history store of the specified HistoryContext.
        /// </summary>
        /// <typeparam name="TContext">The type of the t context.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="dbContextFactory">The database context factory.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory<TContext>(
            this IServiceCollection services,
            Func<IServiceProvider, DbContextOptions<HistoryContext>> dbContextFactory)
            where TContext : HistoryContext
        {
            services.AddTransient<HistoryContext, TContext>(s => (TContext)typeof(TContext)
                .GetConstructor(new[] { typeof(DbContextOptions<HistoryContext>) })
                .Invoke(new object[] { dbContextFactory(s) }));
            services.Add( new ServiceDescriptor(typeof(DbContextOptions<HistoryContext>), dbContextFactory, ServiceLifetime.Singleton));
            services.AddSingleton<IHistoryStore, EntityHistoryStore<TContext>>();
            return services;
        }


    }
}
