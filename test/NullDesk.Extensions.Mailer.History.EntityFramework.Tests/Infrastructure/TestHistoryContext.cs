using Microsoft.EntityFrameworkCore;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.Tests.Infrastructure
{
    public class TestHistoryContext : HistoryContext
    {
        public TestHistoryContext(DbContextOptions<HistoryContext> options) : base(options)
        {
        }
    }
}