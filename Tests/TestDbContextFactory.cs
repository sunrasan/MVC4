using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;

namespace MvcMusicStore.Tests
{
    /// <summary>
    /// Provides SQLite in-memory database contexts for testing.
    /// Each test gets a unique database to ensure isolation.
    /// </summary>
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext CreateSqliteContext()
        {
            // Use a unique in-memory SQLite database per call
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
