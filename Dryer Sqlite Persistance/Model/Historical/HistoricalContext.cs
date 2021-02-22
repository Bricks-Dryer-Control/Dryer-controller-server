using Dryer_Server.Persistance;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    public class HistoricalContext : DbContext
    {
        public HistoricalContext(DbContextOptions<HistoricalContext> options) : base(options) { } 

        public virtual DbSet<ChamberSensorValue> Sensors { get; set; } 
        public virtual DbSet<ChamberControllerState> States { get; set; } 
    }

    public class DesignTimeHistoricalContextFactory : IDesignTimeDbContextFactory<HistoricalContext> 
    { 
        public HistoricalContext CreateDbContext(string[] args) 
        { 
            var builder = new DbContextOptionsBuilder<HistoricalContext>();
            var connectionString = Startup.GetHistoricalConnectionString();
            builder.UseSqlite(connectionString); 
            return new HistoricalContext(builder.Options); 
        } 
    }
}