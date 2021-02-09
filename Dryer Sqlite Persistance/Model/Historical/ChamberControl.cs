using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    [Index("ChamberId", "TimestampUtc", IsUnique = true)]
    public record ChamberControl
    {
        [Key]
        public int id { get; set; }
        public int ChamberId { get; set; }
        public DateTime TimestampUtc { get; set; }
        public bool IsAuto { get; set; }
        public DateTime? AutoControlStartTime { get; set; }
        public virtual ICollection<AutoControlStage> AutoControlStages { get; set; }

    }
}