using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Historical
{
    [Table("State")]
    public record ChamberConvertedState: ChamberConvertedStatus, IChamberStatusHistoricValue
    {

        public int ChamberId { get; set; }
        public DateTime TimestampUtc { get; set;}

        public ChamberConvertedState()
        { }

        public ChamberConvertedState(ChamberConvertedStatus other, int id): base(other)
        {
            ChamberId = id;
            TimestampUtc = DateTime.UtcNow;
        }

        [NotMapped]
        public DateTime TimeUtc => TimestampUtc;
    }
}