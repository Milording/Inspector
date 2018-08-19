using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Inspector
{
    public class ModelContext : DbContext
    {
        public ModelContext()
        {

        }

        public ModelContext(DbContextOptions contextOptions)
        : base(contextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder {DataSource = "inspection.db"};
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }

        public DbSet<SourceState> SourceStates { get; set; }
        public DbSet<InspectedLink> InspectionLink { get; set; }
    }
}
