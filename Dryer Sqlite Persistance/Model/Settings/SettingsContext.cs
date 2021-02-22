using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dryer_Server.Persistance.Model.Settings
{
    public class SettingsContext : DbContext
    {
        public SettingsContext(DbContextOptions<SettingsContext> options) : base(options) { } 

        public virtual DbSet<ChamberSetting> Chamber { get; set; } 
    }

    public class DesignTimeSettingsContextFactory : IDesignTimeDbContextFactory<SettingsContext> 
    { 
        public SettingsContext CreateDbContext(string[] args) 
        { 
            var builder = new DbContextOptionsBuilder<SettingsContext>();
            var connectionString = Startup.GetSettingsConnectionString();
            builder.UseSqlite(connectionString); 
            return new SettingsContext(builder.Options); 
        } 
    }
}