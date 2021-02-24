using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dryer_Server.Interfaces;

namespace Dryer_Server.WebApi
{
    public interface IUiDataKeeper
    {

    }

    public class UiDataKeeper: IUiInterface, IUiDataKeeper
    {
        public void SensorsReceived(int id, DateTime timestampUtc, ChamberSensors values)
        {

        }
        
        public void StatusChanged(int id, DateTime timestampUtc, ChamberConvertedStatus values)
        {

        }
        
        public async Task InitializationFinishedAsync(IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> initializationData, IEnumerable<AdditionalStatus> initializationWents, IEnumerable<(AdditionalStatus Roof, AdditionalStatus Through)> initializationRoofs)
        {

        }
        
        public void WentChanged(int no, int position, int set)
        {

        }
        
        public void RoofThroughChanged(int no, int position, int set)
        {

        }
        
        public void RoofRoofChanged(int no, int position, int set)
        {

        }

    }
}