using Microsoft.EntityFrameworkCore;
using NullDesk.Extensions.Mailer.History.EntityFramework;
using NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer;

namespace Sample.Mailer.Cli.History
{
    public class MailerCliHistoryContext : SqlHistoryContext
    {
        public MailerCliHistoryContext(DbContextOptions<HistoryContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //use alternate schema name
            modelBuilder.HasDefaultSchema("mailerCli");
            base.OnModelCreating(modelBuilder);
        }
    }
}