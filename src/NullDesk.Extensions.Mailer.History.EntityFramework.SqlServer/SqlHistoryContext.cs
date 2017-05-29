using Microsoft.EntityFrameworkCore;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer
{
    /// <summary>
    ///     SQL Server DbContext for Message History.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public abstract class SqlHistoryContext : HistoryContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlHistoryContext" /> class.
        /// </summary>
        protected SqlHistoryContext()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlHistoryContext" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        protected SqlHistoryContext(DbContextOptions<HistoryContext> options) : base(options)
        {
        }
    }
}