using Dryer_Server.Persistance;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    public class HistoricalContext : DbContext
    {
        public HistoricalContext(DbContextOptions<HistoricalContext> options) : base(options) { } 

        public virtual DbSet<ChamberSensorValue> Sensors { get; set; } 
        public virtual DbSet<ChamberConvertedState> States { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChamberSensorValue>()
                .HasKey(c => new { c.ChamberId, c.TimestampUtc });
                
            modelBuilder.Entity<ChamberConvertedState>()
                .HasKey(c => new { c.ChamberId, c.TimestampUtc });
        }
    }

    public class DesignTimeHistoricalContextFactory : IDesignTimeDbContextFactory<HistoricalContext> 
    { 
        public HistoricalContext CreateDbContext(string[] args) 
        { 
            var builder = new DbContextOptionsBuilder<HistoricalContext>();
            builder.UseSqlite("Data Source=./historic.db;Mode=ReadWriteCreate;Cache=Default;Foreign Keys=True"); 
            return new HistoricalContext(builder.Options); 
        } 
    }
}