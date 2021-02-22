using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public interface IDryerHisctoricalValuesPersistance
    {
        IEnumerable<(int id, ChamberControllerStatus status, ChamberSensors sensors)> GetLastValues(IEnumerable<int> ids);
        IEnumerable<IChamberSensorHistoricValue> GetSensorsHistory(int id, DateTime startUtc, DateTime finishUtc);
        void Save(int id, ChamberControllerStatus status);
        void Save(int id, ChamberSensors sensors);
    }
}
