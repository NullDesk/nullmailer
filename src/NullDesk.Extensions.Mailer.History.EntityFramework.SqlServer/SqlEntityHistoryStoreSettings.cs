using Microsoft.EntityFrameworkCore;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer
{
    /// <summary>
    ///     Settings for an entity framework history store using SQL Server.
    /// </summary>
    /// <seealso cref="NullDesk.Extensions.Mailer.Core.StandardHistoryStoreSettings" />
    public class SqlEntityHistoryStoreSettings : StandardHistoryStoreSettings
    {
        /// <summary>
        ///     The SQL connection string for the history context.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="SqlEntityHistoryStoreSettings" /> to
        ///     <see cref="EntityHistoryStoreSettings" />.
        /// </summary>
        /// <param name="sqlSettings">The d.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator EntityHistoryStoreSettings(SqlEntityHistoryStoreSettings sqlSettings)
        {
            return new EntityHistoryStoreSettings
            {
                IsEnabled = sqlSettings.IsEnabled,
                StoreAttachmentContents = sqlSettings.StoreAttachmentContents,
                DbOptions = new DbContextOptionsBuilder<HistoryContext>()
                    .UseSqlServer(sqlSettings.ConnectionString)
                    .Options
            };
        }
    }
}