using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Sample.Mailer.Cli.History
{
    /// <summary>
    ///     Class MailerCliHistoryContextFactory.
    /// </summary>
    /// <remarks>
    ///     The connection details aren't that important, this a work around to allow ef tooling to function correctly from a
    ///     class library.
    /// </remarks>
    public class MailerCliHistoryContextFactory : IDbContextFactory<MailerCliHistoryContext>
    {
        /// <summary>
        ///     Creates a new instance of the  context.
        /// </summary>
        /// <remarks>
        ///     Used by EF CLI tooling
        /// </remarks>
        /// <param name="options">Information about the environment an application is running in.</param>
        public MailerCliHistoryContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<MailerCliHistoryContext>();

            builder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=MailerCliHistory;Trusted_Connection=True;");
            return new MailerCliHistoryContext(builder.Options);
        }
    }
}