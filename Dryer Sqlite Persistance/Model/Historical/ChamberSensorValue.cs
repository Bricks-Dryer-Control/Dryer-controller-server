using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    [Keyless]
    [Table("Sensor")]
    [Index("ChamberId", "TimestampUtc", IsUnique = true)]
    public record ChamberSensorValue: ChamberSensors, IChamberSensorHistoricValue
    {        
        public int ChamberId { get; set; }
        public DateTime TimestampUtc { get; set;}

        public ChamberSensorValue()
        { }

        public ChamberSensorValue(ChamberSensors other, int id): base(other)
        {
            ChamberId = id;
            TimestampUtc = DateTime.UtcNow;
        }

        [NotMapped]
        DateTime IChamberSensorHistoricValue.TimeUtc => TimestampUtc;

        [NotMapped]
        ChamberSensors IChamberSensorHistoricValue.Value => this;
    }
}