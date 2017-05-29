using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NullDesk.Extensions.Mailer.Core;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer;

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
        /// <param name="connectionString">The connection string for the Sql database.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory<TContext>(
            this IServiceCollection services,
            string connectionString)
        where TContext : SqlHistoryContext
        {
            var dbContextOptions = new DbContextOptionsBuilder<HistoryContext>()
                .UseSqlServer(connectionString)
                .Options;
            return services.AddMailerHistory<TContext>(dbContextOptions);
        }

        /// <summary>
        ///     Adds an EntityFramwork mailer history store using a default SqlHistoryContext.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="connectionString">The connection string for the Sql database.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddMailerHistory(
            this IServiceCollection services,
            string connectionString)
        {
            var dbContextOptions = new DbContextOptionsBuilder<HistoryContext>()
                .UseSqlServer(connectionString)
                .Options;
            return services.AddMailerHistory<SqlHistoryContext>(dbContextOptions);
        }

      
    }
}
