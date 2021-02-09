using System;

namespace Dryer_Server.Interfaces
{
    public interface IAppSetting
    {
        DateTime CreationTimeUtc { get; set; }
        string ListenChamberPort { get; set; }
        string ActuatorControlsPort { get; set; }
        int MaxActuatorsWorking { get; set; }
        int DirectionSensorAddress { get; set; }
        int DirectionSensorInputNo { get; set; }
        TimeSpan HistoryInterval { get; set; }
        TimeSpan QuickHistoryShowHistoryTime { get; set; }
        TimeSpan QuickHistoryShowFutureTime { get; set; }
        TimeSpan ActuatorSetingTimeout { get; set; }
    }
}
