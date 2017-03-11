using Microsoft.EntityFrameworkCore;

namespace NullDesk.Extensions.Mailer.History.EntityFramework
{
    /// <summary>
    ///     Base DbContext for Message History.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public abstract class HistoryContext : DbContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HistoryContext" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        protected HistoryContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        ///     Gets or sets the history items.
        /// </summary>
        /// <value>The history items.</value>
        public DbSet<EntityHistoryDeliveryItem> MessageHistory { get; set; }


    }
}