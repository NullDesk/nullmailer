using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests.Infrastructure
{
    /// <summary>
    ///     Class TestSqlHistoryContextFactory.
    /// </summary>
    /// <remarks>
    ///     The connection details aren't that important, this a work around to allow ef tooling to function correctly from a
    ///     class library.
    /// </remarks>
    public class TestSqlHistoryContextFactory : IDesignTimeDbContextFactory<TestSqlHistoryContext>
    {
        /// <summary>
        ///     Creates a new instance of the  context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>An instance of <typeparamref name="TContext" />.</returns>
        /// <remarks>Used by EF CLI tooling</remarks>
        public TestSqlHistoryContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<HistoryContext>();

            builder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=NullMailerHistory;Trusted_Connection=True;");
            return new TestSqlHistoryContext(builder.Options);
        }
    }
}