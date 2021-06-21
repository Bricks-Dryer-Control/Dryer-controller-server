using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    [Table("Sensor")]
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