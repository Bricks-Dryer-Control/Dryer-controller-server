using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dryer_Server.Interfaces
{
    public interface IUiInterface
    {
        void SensorsReceived(int id, DateTime timestampUtc, ChamberSensors values);
        void StatusChanged(int id, DateTime timestampUtc, ChamberConvertedStatus values);
        Task InitializationFinishedAsync(IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> initializationData, IEnumerable<AdditionalStatus> initializationWents, IEnumerable<(AdditionalStatus Roof, AdditionalStatus Through)> initializationRoofs);
        void WentChanged(int no, int position, int set);
        void RoofThroughChanged(int no, int position, int set);
        void RoofRoofChanged(int no, int position, int set);
    }
}
