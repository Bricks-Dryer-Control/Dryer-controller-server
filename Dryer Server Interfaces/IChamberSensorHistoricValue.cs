using System;

namespace Dryer_Server.Interfaces
{
    public interface IChamberSensorHistoricValue
    {
        DateTime TimeUtc { get; }
        ChamberSensors Value { get; }
    }
}