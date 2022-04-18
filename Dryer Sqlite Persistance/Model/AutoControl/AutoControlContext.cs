using Dryer_Server.Persistance;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.AutoControl
{
    public class AutoControlContext : DbContext
    {
        public AutoControlContext(DbContextOptions<AutoControlContext> options) : base(options) { } 

        public virtual DbSet<DbAutoControlItem> Sets { get; set; } 
        public virtual DbSet<DbAutoControl> Definitions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbAutoControl>()
                .HasMany(d => d.Sets)
                .WithOne(i => i.Definition)
                .HasPrincipalKey(d => d.Id)
                .HasForeignKey(i => i.DefinitionId);
        }
    }

    public class DesignTimeHistoricalContextFactory : IDesignTimeDbContextFactory<AutoControlContext> 
    { 
        public AutoControlContext CreateDbContext(string[] args) 
        { 
            var builder = new DbContextOptionsBuilder<AutoControlContext>();
            builder.UseSqlite("Data Source=./automatic.db;Mode=ReadWriteCreate;Cache=Default;Foreign Keys=True"); 
            return new AutoControlContext(builder.Options); 
        } 
    }
}