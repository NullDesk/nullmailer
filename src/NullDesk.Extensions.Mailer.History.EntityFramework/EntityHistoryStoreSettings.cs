using Microsoft.EntityFrameworkCore;
using NullDesk.Extensions.Mailer.Core;

namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    ///     Settings for Entity Framework History Stores.
    /// </summary>
    /// <seealso cref="StandardHistoryStoreSettings" />
    public class EntityHistoryStoreSettings : StandardHistoryStoreSettings
    {
        /// <summary>
        ///     The database context options.
        /// </summary>
        /// <value>The database context options.</value>
        public DbContextOptions<HistoryContext> DbOptions { get; set; }
    }
}