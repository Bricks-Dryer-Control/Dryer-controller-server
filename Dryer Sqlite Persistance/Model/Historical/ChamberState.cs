using System;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    [Keyless][Index("ChamberId", "TimestampUtc", IsUnique = true)]
    public record ChamberState
    {        
        public int ChamberId { get; set; }
        public DateTime TimestampUtc { get; set;}
        public int Inflow { get; set; }
        public int Outflow { get; set; }
        public int Throughflow { get; set; }
    }
}