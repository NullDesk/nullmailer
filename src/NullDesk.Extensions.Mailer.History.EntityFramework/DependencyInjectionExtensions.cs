using System;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    /// Class DependencyInjectionExtensions.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds a mailer history store of the specified HistoryContext.
        /// </summary>
        /// <typeparam name="TContext">The type of the History DbContext.</typeparam>
        /// <param name="services">The services collection.</param>
        /// <param name="entityHistorySettings">The entity history settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory<TContext>(
            this IServiceCollection services,
            EntityHistoryStoreSettings entityHistorySettings)
        where TContext : HistoryContext
        {
            
            services.AddSingleton(entityHistorySettings);
            services.AddSingleton<IHistoryStore, EntityHistoryStore<TContext>>();
            return services;
        }

        /// <summary>
        /// Adds a mailer history store of the specified HistoryContext.
        /// </summary>
        /// <typeparam name="TContext">The type of the t context.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="historySettings">The history settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory<TContext>(
            this IServiceCollection services,
            Func<IServiceProvider, EntityHistoryStoreSettings> historySettings)
            where TContext : HistoryContext
        {
            services.Add(new ServiceDescriptor(typeof(EntityHistoryStoreSettings), historySettings, ServiceLifetime.Singleton));
            services.AddSingleton<IHistoryStore, EntityHistoryStore<TContext>>();
            return services;
        }
    }
}
