﻿using Microsoft.EntityFrameworkCore;

namespace NullDesk.Extensions.Mailer.History.EntityFramework.SqlServer.Tests.Infrastructure
{
    public class TestSqlHistoryContext : SqlHistoryContext
    {
        public TestSqlHistoryContext(DbContextOptions options) : base(options)
        {
        }
    }
}