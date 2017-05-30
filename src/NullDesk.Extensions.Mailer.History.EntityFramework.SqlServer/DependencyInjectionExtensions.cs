using System;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    ///     Class DependencyInjectionExtensions.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        ///     Adds an EntityFramework mailer history store for the specified HistoryContext type.
        /// </summary>
        /// <typeparam name="TContext">The type of the history DbContext.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="sqlHistorySettings">The SQL history store settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory<TContext>(
            this IServiceCollection services,
            SqlEntityHistoryStoreSettings sqlHistorySettings)
        where TContext : SqlHistoryContext
        {
            //implicit conversion
            return services.AddMailerHistory<TContext>((EntityHistoryStoreSettings)sqlHistorySettings);
        }

        /// <summary>
        /// Adds an EntityFramework mailer history store for the specified HistoryContext type.
        /// </summary>
        /// <typeparam name="TContext">The type of the history DbContext.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="sqlHistorySettings">The SQL history store settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory<TContext>(
            this IServiceCollection services,
            Action<SqlEntityHistoryStoreSettings> sqlHistorySettings)
            where TContext : SqlHistoryContext
        {
            var settings = new SqlEntityHistoryStoreSettings();
            sqlHistorySettings(settings);
            
            //implicit conversion
            return services.AddMailerHistory<TContext>((EntityHistoryStoreSettings)settings);
        }

        /// <summary>
        ///      Adds a mailer history store of the specified HistoryContext.
        /// </summary>
        /// <typeparam name="TContext">The type of the t context.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="sqlHistorySettings">Function to obtain sql history settings.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory<TContext>(
            this IServiceCollection services,
            Func<IServiceProvider, SqlEntityHistoryStoreSettings> sqlHistorySettings)
            where TContext : HistoryContext
        {
            return services.AddMailerHistory<TContext>(s => (EntityHistoryStoreSettings)sqlHistorySettings(s));
        }

    }
}
