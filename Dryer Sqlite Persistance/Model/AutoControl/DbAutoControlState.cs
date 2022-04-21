using Dryer_Server.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dryer_Server.Persistance.Model.AutoControl
{
    [Table("AutoControlStates")]
    public record DbAutoControlState
    {
        [Key]
        public int Id { get; set; }
        public int ChamberId { get; set; }
        public DateTime StartUtc { get; set; }
        public DbAutoControl AutoControl { get; set; }

        public DbAutoControlState()
        {}

        public DbAutoControlState(int chamberId, IAutoControl autoControl, DbAutoControl dbAutoControl)
        {
            ChamberId = chamberId;
            StartUtc = autoControl.StartDateUtc;
            AutoControl = dbAutoControl;
        }
    }
}
