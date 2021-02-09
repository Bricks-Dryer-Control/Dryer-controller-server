using System;
using Dryer_Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dryer_Server.Persistance.Model.Settings
{
    [Keyless]
    [Index("CreationTimeUtc", IsUnique = true)]
    public record AppSetting : IAppSetting
    {
        public DateTime CreationTimeUtc { get; set; }
        public string ListenChamberPort { get; set; }
        public string ActuatorControlsPort { get; set; }
        public int MaxActuatorsWorking { get; set; }
        public int DirectionSensorAddress { get; set; }
        public int DirectionSensorInputNo { get; set; }
        public TimeSpan HistoryInterval { get; set; }
        public TimeSpan QuickHistoryShowHistoryTime { get; set; }
        public TimeSpan QuickHistoryShowFutureTime { get; set; }
        public TimeSpan ActuatorSetingTimeout { get; set; }
    }
}