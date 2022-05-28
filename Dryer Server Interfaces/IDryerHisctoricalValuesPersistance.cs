using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public interface IDryerHisctoricalValuesPersistance: IDisposable
    {
        IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> GetLastValues(IEnumerable<int> ids);
        IEnumerable<IHistoricValue> GetSensorsHistory(int id, DateTime startUtc, DateTime finishUtc);
        IEnumerable<IHistoricValue> GetStatusHistory(int id, DateTime startUtc, DateTime finishUtc);
        void Save(int id, ChamberConvertedStatus status);
        void Save(int id, ChamberSensors sensors);
    }
}
