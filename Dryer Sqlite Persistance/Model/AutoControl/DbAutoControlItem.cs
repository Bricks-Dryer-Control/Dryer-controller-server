using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.AutoControl
{
    
    [Table("Sets")]
    [Index("DefinitionId", "Time", IsUnique = true)]
    public record DbAutoControlItem
    {
        [Key]
        public int Id { get; set; }
        public int DefinitionId { get;  set; }
        public virtual DbAutoControl Definition { get; set; }
        public TimeSpan Time { get; set; }
        public float Temperature { get; set; }
        public int InFlow { get; set; }
        public int OutFlow { get; set; }
        public int ThroughFlow { get; set; }

        public DbAutoControlItem()
        { }
        public DbAutoControlItem(AutoControlItem autoControlItem)
        {
            Time = autoControlItem.Time;
            Temperature = autoControlItem.Temperature;
            InFlow = autoControlItem.InFlow;
            OutFlow = autoControlItem.OutFlow;
            ThroughFlow = autoControlItem.ThroughFlow;
        }

        public AutoControlItem ToAutoControlItem()
        {
            return new AutoControlItem
            {
                Time = Time,
                Temperature = Temperature,
                InFlow = InFlow,
                OutFlow = OutFlow,
                ThroughFlow = ThroughFlow,
            };
        }
    }
}