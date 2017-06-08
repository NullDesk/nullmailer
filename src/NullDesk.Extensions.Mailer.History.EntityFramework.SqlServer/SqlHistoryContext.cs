using Microsoft.EntityFrameworkCore;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer
{
    /// <summary>
    ///     SQL Server DbContext for Message History.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class SqlHistoryContext : HistoryContext
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
        public SqlHistoryContext(DbContextOptions<HistoryContext> options) : base(options)
        {
        }

        /// <summary>
        ///     Initializes the database.
        /// </summary>
        /// <remarks>Used to run migrations.</remarks>
        public override void InitializeDatabase()
        {
            Database.Migrate();
        }
    }
}