using System;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    [Keyless][Index("ChamberId", "TimestampUtc", IsUnique = true)]
    public record ChamberSensor
    {        
        public int ChamberId { get; set; }
        public DateTime TimestampUtc { get; set;}
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}