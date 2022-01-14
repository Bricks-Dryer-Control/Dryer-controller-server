using Dryer_Server.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dryer_Server.Persistance.Model.AutoControl;

namespace Dryer_Sqlite.Persistance.Model.AutoControl
{
    [Table("TimeBasedAutoControls")]
    public record DbTimeBasedAutoControl
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartMoment { get; set; }
        public DbAutoControl AutoControl { get; set; }
        [ForeignKey("ChamberConfiguration")]
        public int ChamberId { get; set; }


        public DbTimeBasedAutoControl()
        {}
        public DbTimeBasedAutoControl(ITimeBasedAutoControl timeBasedAutoControl)
        {
            AutoControl =new DbAutoControl(timeBasedAutoControl.AutoControl);
            ChamberId = timeBasedAutoControl.AutoControlledChamber.Id;
            StartMoment = timeBasedAutoControl.StartMoment;
        }

    }
}
