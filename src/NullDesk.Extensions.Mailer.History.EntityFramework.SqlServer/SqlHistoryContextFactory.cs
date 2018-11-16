using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer
{
    /// <summary>
    ///     Class SqlHistoryContextFactory.
    /// </summary>
    public class SqlHistoryContextFactory : IDesignTimeDbContextFactory<SqlHistoryContext>
    {
        /// <summary>
        ///     Creates a new instance of the  context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>An instance of <typeparamref name="TContext" />.</returns>
        /// <remarks>Used by EF CLI tooling</remarks>
        public SqlHistoryContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<HistoryContext>();

            builder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=MailerCliHistory;Trusted_Connection=True;");
            return new SqlHistoryContext(builder.Options);
        }
    }
}