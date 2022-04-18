using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dryer_Server.Persistance.Model.Settings
{
    public class SettingsContext : DbContext
    {
        public SettingsContext(DbContextOptions<SettingsContext> options) : base(options) { } 

        public virtual DbSet<ChamberSetting> Chamber { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChamberSetting>()
                .HasKey(c => new { c.Id, c.CreationTimeUtc });
        }
    }

    public class DesignTimeSettingsContextFactory : IDesignTimeDbContextFactory<SettingsContext> 
    { 
        public SettingsContext CreateDbContext(string[] args) 
        { 
            var builder = new DbContextOptionsBuilder<SettingsContext>();
            builder.UseSqlite("Data Source=./settings.db;Mode=ReadWriteCreate;Cache=Default;Foreign Keys=True");
            return new SettingsContext(builder.Options); 
        } 
    }
}