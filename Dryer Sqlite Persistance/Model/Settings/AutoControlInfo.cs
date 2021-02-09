using System;
using System.ComponentModel.DataAnnotations;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Settings
{
    [Index("Name", "CreationTimeUtc", IsUnique = true)]
    public record AutoControlInfo// : IAutoControlInfo
    {
        [Key]
        public int id { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public string Name {get;set;}
        public AutoControlType ControlType {get;set;}
        
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