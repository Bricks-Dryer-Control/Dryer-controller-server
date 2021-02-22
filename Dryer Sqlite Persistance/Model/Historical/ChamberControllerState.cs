using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    [Keyless]
    [Table("State")]
    [Index("ChamberId", "TimestampUtc", IsUnique = true)]
    public record ChamberControllerState: ChamberControllerStatus
    {        
        public int ChamberId { get; set; }
        public DateTime TimestampUtc { get; set;}

        public ChamberControllerState()
        { }

        public ChamberControllerState(ChamberControllerStatus other, int id): base(other)
        {
            ChamberId = id;
            TimestampUtc = DateTime.UtcNow;
        }
    }
}