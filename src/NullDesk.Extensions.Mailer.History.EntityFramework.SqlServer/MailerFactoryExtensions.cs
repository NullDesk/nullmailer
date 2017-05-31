using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer
{
    /// <summary>
    /// Class MailerFactoryExtensions.
    /// </summary>
    public static class MailerFactoryExtensions
    {
        /// <summary>
        /// Registers the default history store as a SQL history store using the supplied settings.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="sqlHistorySettings">The SQL history settings.</param>
        public static void RegisterDefaultSqlHistory(this MailerFactory factory, SqlEntityHistoryStoreSettings sqlHistorySettings)
        {
            factory.DefaultHistoryStore = new EntityHistoryStore<SqlHistoryContext>(sqlHistorySettings);
        }

        /// <summary>
        /// Registers the default history store as a SQL history store using the supplied settings and DbContext type.
        /// </summary>
        /// <typeparam name="TContext">The type of the t context.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="sqlHistorySettings">The SQL history settings.</param>
        public static void RegisterDefaultSqlHistory<TContext>(this MailerFactory factory,
            SqlEntityHistoryStoreSettings sqlHistorySettings)
            where TContext : SqlHistoryContext
        {
            factory.DefaultHistoryStore = new EntityHistoryStore<TContext>(sqlHistorySettings);
        }


    }
}
