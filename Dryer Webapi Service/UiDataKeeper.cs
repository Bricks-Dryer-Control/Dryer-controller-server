using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dryer_Server.Interfaces;
using Dryer_Server.WebApi.Model;

namespace Dryer_Server.WebApi
{
    public interface IUiDataKeeper
    {
        IEnumerable<ChamberInfo> GetChambers();
        ChamberInfo GetChamber(int no);
        AdditionalInfo GetAdditionalInfo();
    }

    public class UiDataKeeper: IUiInterface, IUiDataKeeper
    {
        private static ChamberInfo[] chambers = null;
        private static AdditionalInfo additionalInfo = null;

        private static ChamberConvertedStatus DefaultStatus { get; } = new()
        {
            Working = ChamberConvertedStatus.WorkingStatus.off,
            IsAuto = false,
            QueuePosition = null,
            InFlowPosition = 0,
            OutFlowPosition = 0,
            ThroughFlowPosition = 0,
            InFlowSet = 0,
            OutFlowSet = 0,
            ThroughFlowSet = 0,
            IsListening = false,
        };

        private static ChamberSensors DefaultSensors { get; } = new()
        {
            Humidity = 0F,
            Temperature = 0F,
        };

        public void SensorsReceived(int id, DateTime timestampUtc, ChamberSensors values)
        {
            var chamber = chambers[id -1];
            chamber.Humidity = values.Humidity;
            chamber.Temperature = values.Temperature;
            chamber.ReadingTime = timestampUtc;
        }
        
        public void StatusChanged(int id, DateTime timestampUtc, ChamberConvertedStatus values)
        {
            var chamber = chambers[id -1];
            chamber.ReadingTime = timestampUtc;

            if (values.Working == ChamberConvertedStatus.WorkingStatus.error) 
            {
                chamber.Status.Working = ChamberConvertedStatus.WorkingStatus.error;
                return;
            }

            chamber.ActualActuators = new ChamberValues
            {
                InFlow = values.InFlowPosition,
                OutFlow = values.OutFlowPosition,
                ThroughFlow = values.ThroughFlowPosition,
            };
            chamber.SetActuators = new ChamberValues
            {
                InFlow = values.InFlowSet,
                OutFlow = values.OutFlowSet,
                ThroughFlow = values.ThroughFlowSet,
            };
            chamber.Status = new ChamberStatus
            {
                IsAuto = false,
                QueuePosition = values.QueuePosition,
                Working = values.Working,
                IsActive = values.IsListening,
            };
        }
        
        public void ActiveChanged(int id, bool value)
        {
            var chamber = chambers[id -1];
            chamber.Status.IsActive = value;
            chamber.ReadingTime = DateTime.UtcNow;
        }

        public void WentChanged(int no, int position, int set, int? queuePosition, ChamberConvertedStatus.WorkingStatus status)
        {
            var went = additionalInfo.Wents[no - 1];
            SetAdditionalStatus(went, position, set, queuePosition, status);
        }

        public void RoofThroughChanged(int no, int position, int set, int? queuePosition, ChamberConvertedStatus.WorkingStatus status)
        {
            var roofThrough = additionalInfo.Roofs[no - 1].through;
            SetAdditionalStatus(roofThrough, position, set, queuePosition, status);
        }
        
        public void RoofRoofChanged(int no, int position, int set, int? queuePosition, ChamberConvertedStatus.WorkingStatus status)
        {
            var roofRoof = additionalInfo.Roofs[no - 1].roof;
            SetAdditionalStatus(roofRoof, position, set, queuePosition, status);
        }

        private static void SetAdditionalStatus(Model.AdditionalStatus toChange, int position, int set, int? queuePosition, ChamberConvertedStatus.WorkingStatus status)
        {
            toChange.actualValue = position;
            toChange.setValue = set;
            toChange.status = new ChamberStatus
            {
                IsAuto = false,
                QueuePosition = queuePosition,
                Working = status,
            };
        }

        public async Task InitializationFinishedAsync(IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> initializationData, IEnumerable<Interfaces.AdditionalStatus> initializationWents, IEnumerable<(Interfaces.AdditionalStatus Roof, Interfaces.AdditionalStatus Through)> initializationRoofs)
        {
            var initializeAdditionalInfo = Task.Run(() => InitializeAdditionalInfo(initializationWents, initializationRoofs));
            var initializeChambers = Task.Run(() => InitializeChambers(initializationData));

            await initializeAdditionalInfo;
            await initializeChambers;
        }

        private void InitializeChambers(IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> initializationData)
        {
            var chamberList = initializationData
                .Select(((int id, ChamberConvertedStatus status, ChamberSensors sensors) x) 
                    => new ChamberInfo(x.id, x.status ?? DefaultStatus, x.sensors ?? DefaultSensors))
                .ToList();
            
            var maxChamber = chamberList.Select(c => c.No).Max();
            chambers = new ChamberInfo[maxChamber];
            for (var i = 0; i < maxChamber; i++)
                chambers[i] = chamberList.FirstOrDefault(c => c.No == i + 1) ?? new ChamberInfo(i+1, DefaultStatus, DefaultSensors);
        }

        private void InitializeAdditionalInfo(IEnumerable<Interfaces.AdditionalStatus> initializationWents, IEnumerable<(Interfaces.AdditionalStatus Roof, Interfaces.AdditionalStatus Through)> initializationRoofs)
        {
            var wents = initializationWents.Select(w => new Model.AdditionalStatus(w)).ToArray();
            var roofs = initializationRoofs
                .Select(r => new AdditionalRoofInfo
                {
                    roof = new Model.AdditionalStatus(r.Roof),
                    through = new Model.AdditionalStatus(r.Through),
                })
                .ToArray();
            additionalInfo = new AdditionalInfo
            {
                Roofs = roofs,
                Wents = wents,
            };
        }

        public IEnumerable<ChamberInfo> GetChambers()
        {
            return chambers;
        }

        public ChamberInfo GetChamber(int no)
        {
            return chambers[no - 1];
        }

        public AdditionalInfo GetAdditionalInfo()
        {
            return additionalInfo;
        }
    }
}