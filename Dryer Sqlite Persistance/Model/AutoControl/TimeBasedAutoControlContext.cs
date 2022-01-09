using Microsoft.EntityFrameworkCore;

namespace Dryer_Sqlite_Persistance.Model.AutoControl
{
    public class TimeBasedAutoControlContext: DbContext
    {

        public TimeBasedAutoControlContext(DbContextOptions<TimeBasedAutoControlContext> timeBasedAutoControlOptions)
        {
        }

        public virtual DbSet<DbTimeBasedAutoControl> TimeBasedAutoControls { get; set; }

    }
}
