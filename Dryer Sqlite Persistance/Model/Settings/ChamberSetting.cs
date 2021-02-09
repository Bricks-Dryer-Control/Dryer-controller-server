using System;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Settings
{
    [Keyless]
    [Index("ChamberId", "CreationTimeUtc", IsUnique = true)]
    public record ChamberSetting : IChamberSetting
    {
        public int ChamberId { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public int? SensorId { get; set; }
        public int InflowIndex { get; set; }
        public int OutflowIndex { get; set; }
        public int ThroughflowIndex { get; set; }
        public double Offset { get; set; }
        public double K { get; set; }
        public double Ti { get; set; }
        public double Td { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double MinAutoControlInflow { get; set; }
        public double MaxAutoControlInflow { get; set; }
        public double MinAutoControlOutflow { get; set; }
        public double MaxAutoControlOutflow { get; set; }
        public double MinAutoControlThroughflow { get; set; }
        public double MaxAutoControlThroughflow { get; set; }
        public bool IsDeleted {get; set;}

        public static ChamberSetting Get(IChamberSetting other)
        {
            if (other is ChamberSetting chamberSetting)
                return chamberSetting;

            return new ChamberSetting
            {
                ChamberId = other.ChamberId,
                SensorId = other.SensorId,
                IsDeleted = false,
                CreationTimeUtc = other.CreationTimeUtc == default ? DateTime.UtcNow : other.CreationTimeUtc,
                InflowIndex = other.InflowIndex,
                OutflowIndex = other.OutflowIndex,
                ThroughflowIndex = other.ThroughflowIndex,
                Offset = other.Offset,
                K = other.K,
                Ti = other.Ti,
                Td = other.Td,
                A = other.A,
                B = other.B,
            };
        }
    }
}