using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer
{
    /// <summary>
    ///     Class SqlHistoryContextFactory.
    /// </summary>
    public class SqlHistoryContextFactory : IDbContextFactory<SqlHistoryContext>
    {
        /// <summary>
        ///     Creates a new instance of the  context.
        /// </summary>
        /// <remarks>
        ///     Used by EF CLI tooling
        /// </remarks>
        /// <param name="options">Information about the environment an application is running in.</param>
        public SqlHistoryContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<HistoryContext>();

            builder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=MailerCliHistory;Trusted_Connection=True;");
            return new SqlHistoryContext(builder.Options);
        }
    }
}