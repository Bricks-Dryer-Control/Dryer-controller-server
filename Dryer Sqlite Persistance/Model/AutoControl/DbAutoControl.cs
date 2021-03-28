using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.AutoControl
{
    [Table("Definitions")]
    [Index("Name", IsUnique = false)]
    public record DbAutoControl
    {
        [Key]
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public TimeSpan TimeToSet { get; set; }
        public float TemperatureDifference { get; set; }
        public int ControlDifference { get; set; }
        public AutoControlType ControlType { get; set; }
        public float Kp { get; set; }
        public float Ki { get; set; }
        public int MinInFlow { get; set; }
        public int MaxInFlow { get; set; }
        public int MinOutFlow { get; set; }
        public int MaxOutFlow { get; set; }
        public float Percent { get; set; }
        public int Offset { get; set; }
        public virtual ICollection<DbAutoControlItem> Sets { get; set; }

        internal Interfaces.AutoControl ToAutoControl()
        {
            return new Interfaces.AutoControl
            {
                Name = Name,
                ControlDifference = ControlDifference,
                ControlType = ControlType,
                TemperatureDifference = TemperatureDifference,
                Ki = Ki,
                Kp = Kp,
                MaxInFlow = MaxInFlow,
                MaxOutFlow = MaxOutFlow,
                MinInFlow = MinInFlow,
                MinOutFlow = MinOutFlow,
                Offset = Offset,
                Percent = Percent,
                TimeToSet = TimeToSet,
                Sets = Sets.Select(i => i.ToAutoControlItem()).ToList()
            };
        }
        public DbAutoControl()
        { }

        public DbAutoControl(Interfaces.AutoControl autoControl)
        {
            Deleted = false;
            Name = autoControl.Name;
            TimeToSet = autoControl.TimeToSet;
            TemperatureDifference = autoControl.TemperatureDifference;
            ControlDifference = autoControl.ControlDifference;
            ControlType = autoControl.ControlType;
            Kp = autoControl.Kp;
            Ki = autoControl.Ki;
            MinInFlow = autoControl.MinInFlow;
            MaxInFlow = autoControl.MaxInFlow;
            MinOutFlow = autoControl.MinOutFlow;
            MaxOutFlow = autoControl.MaxOutFlow;
            Percent = autoControl.Percent;
            Offset = autoControl.Offset;
            Sets = autoControl.Sets.Select(i => new DbAutoControlItem(i)).ToList();
        }
    }
}