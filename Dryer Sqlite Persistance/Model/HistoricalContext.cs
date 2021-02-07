using Dryer_Server.Persistance;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model
{
    public class HistoricalContext : DbContext
    {
        public HistoricalContext(DbContextOptions<HistoricalContext> options) : base(options) { } 

        public virtual DbSet<ChamberSensor> ChamberSensors { get; set; } 
        //public virtual DbSet<ChamberState> CahmberStates { get; set; } 
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