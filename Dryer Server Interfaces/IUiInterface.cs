using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dryer_Server.Interfaces
{
    public interface IUiInterface
    {
        void SensorsReceived(int id, DateTime timestampUtc, ChamberSensors values);
        void StatusChanged(int id, DateTime timestampUtc, ChamberConvertedStatus values);
        void ActiveChanged(int id, bool value);
        Task InitializationFinishedAsync(IEnumerable<ChamberInitializationData> initializationData, int wentQty, int roofQty);
        void WentChanged(int no, int position, int set, int? queuePosition, ChamberConvertedStatus.WorkingStatus status);
        void RoofThroughChanged(int no, int position, int set, int? queuePosition, ChamberConvertedStatus.WorkingStatus status);
        void RoofRoofChanged(int no, int position, int set, int? queuePosition, ChamberConvertedStatus.WorkingStatus status);
        void AutoControlChanged(int id, IAutoControl autoControl);
    }
}
