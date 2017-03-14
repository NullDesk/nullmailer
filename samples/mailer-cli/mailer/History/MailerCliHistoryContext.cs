using Microsoft.EntityFrameworkCore;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer;

namespace Sample.Mailer.Cli.History
{
    public class MailerCliHistoryContext: SqlHistoryContext
    {

        public MailerCliHistoryContext(DbContextOptions options) : base(options)
        {
        }
    }
}
