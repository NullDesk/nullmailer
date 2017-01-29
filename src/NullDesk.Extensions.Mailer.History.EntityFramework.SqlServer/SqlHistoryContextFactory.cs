using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer
{

    /// <summary>
    /// Class SqlHistoryContextFactory.
    /// </summary>
    /// <remarks>
    /// The connection details aren't that important, this a work around to allow ef tooling to function correctly from a class library.
    /// </remarks>
    public class SqlHistoryContextFactory : IDbContextFactory<SqlHistoryContext>
    {
        /// <summary>
        /// Creates a new instance of the  context.
        /// </summary>
        /// <remarks>
        /// Used by EF CLI tooling
        /// </remarks>
        /// <param name="options">Information about the environment an application is running in.</param>
        public SqlHistoryContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<SqlHistoryContext>();
 
            builder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=NullMailerHistory;Trusted_Connection=True;");
            return new SqlHistoryContext(builder.Options);
        }
    }
}
