using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model
{
    [Keyless][Index("Chamber", "Time", IsUnique = true)]
    public record ChamberSensor
    {
        [Column("Chamber")]
        public int ChamberId { get; set; }
        [Column("Time")]
        public DateTime TimestampUtc { get; set;}
        [Column("Temp")]
        public double Temperature { get; set; }
        [Column("Humd")]
        public double Humidity { get; set; }
    }
}