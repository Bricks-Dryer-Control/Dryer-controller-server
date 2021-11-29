using Dryer_Server.Interfaces;
using Dryer_Server.Serial_Modbus_Agent;
using Dryer_Server.Persistance;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Dryer_Server.AutomaticControl;

namespace Dryer_Server.Core
{
    public partial class Main : IMainController, IDisposable
    {
        private readonly IUiInterface ui;
        private readonly SerialModbusChamberListener modbusListener;
        private readonly IDryerConfigurationPersistance configurationPersistance;
        private readonly IDryerHisctoricalValuesPersistance hisctoricalPersistance;
        private readonly IModbusControllerCommunicator controllersCommunicator;
        private readonly IAutoControlPersistance autoControlPersistence;
        private Dictionary<int, AutoControlledChamber> ChambersDictionary { get; } = new Dictionary<int, AutoControlledChamber>();

        public void StartAutoControl(int chamberId, string autoControlName, TimeSpan startingPoint, TimeSpan checkingDelay)
        {
            var chamber = ChambersDictionary[chamberId];
            var autoControl = autoControlPersistence.GetControlWithItems(autoControlName);
            var timeBasedAutoControl = new TimeBasedAutoControl(checkingDelay, startingPoint, autoControl, chamber);
        }

        private Dictionary<int, RoofConfig> Roofs { get; } = new Dictionary<int, RoofConfig>
        {
            {1, new RoofConfig{ No = 1, RoofNo = 4, ThroughNo = 5 }},
            {2, new RoofConfig{ No = 2, RoofNo = 6, ThroughNo = 7 }},
            {3, new RoofConfig{ No = 3, RoofNo = 8, ThroughNo = 9 }},
            {4, new RoofConfig{ No = 4, RoofNo = 15, ThroughNo = 14 }},
        };
        private Dictionary<int, int> Wents {get;} = new Dictionary<int, int>
        {
            {1, 16},
            {2, 17},
        };

        public Main(IUiInterface ui)
        {
            this.ui = ui;
            modbusListener = new SerialModbusChamberListener("COM10");
            var persistance = new SqlitePersistanceManager();
            configurationPersistance = persistance;
            hisctoricalPersistance = persistance;
            autoControlPersistence = persistance;
            controllersCommunicator = new ControllersCommunicator("COM12");
        }

        public async Task InitializeAsync()
        {
            var chambers = configurationPersistance.GetChamberConfigurations();
            var ids = chambers.Select(c => c.Id).ToList();

            var lastValues = hisctoricalPersistance.GetLastValues(ids);
            var initWents = Wents.Select(_ => new AdditionalStatus());
            var initRoofs = Roofs.Select(_ => (Roof: new AdditionalStatus(), Through: new AdditionalStatus()));

            foreach (var chamberSettings in chambers)
            {
                var aditional = new AdditionalConfig
                {
                    RoofRoof = Roofs.Values
                        .Where(r => r.RoofNo == chamberSettings.Id)
                        .FirstOrDefault()?.No,
                    RoofThrough = Roofs.Values
                        .Where(r => r.ThroughNo == chamberSettings.Id)
                        .FirstOrDefault()?.No,
                    Went = Wents
                        .Where(kv => kv.Value == chamberSettings.Id)
                        .Select(kv => kv.Key)
                        .FirstOrDefault()
                };

                var chamber = new AutoControlledChamber(chamberSettings, this, aditional);
                if (chamberSettings.SensorId.HasValue)
                    modbusListener.Add(chamberSettings.SensorId.Value, chamber);

                var lastChamberValues = lastValues.FirstOrDefault(lv => lv.id == chamberSettings.Id);
                var isActive = lastChamberValues.status?.IsListening ?? false;
                controllersCommunicator.Register(chamberSettings, isActive, chamber);
                ChambersDictionary.Add(chamberSettings.Id, chamber);
            }
            
            foreach (var v in lastValues)
            {
                var chamber = ChambersDictionary[v.id];
                if (v.status != null) 
                {
                    chamber.Sets.InFlow = v.status.InFlowSet;
                    chamber.Sets.OutFlow = v.status.OutFlowSet;
                    chamber.Sets.ThroughFlow = v.status.ThroughFlowSet;
                }
            }

            await ui.InitializationFinishedAsync(lastValues, initWents, initRoofs);
        }

        public void Start()
        {
            modbusListener.Start();
            controllersCommunicator.Start();
        }

        public void Stop()
        {
            modbusListener.Stop();
            controllersCommunicator.Stop();
        }

        private void HandleExceptions(IEnumerable<Exception> exs)
        {
            foreach (var e in exs)
                Trace.WriteLine(e);
        }

        public void Dispose()
        {
            controllersCommunicator?.Dispose();
            modbusListener?.Dispose();
            hisctoricalPersistance?.Dispose();
            configurationPersistance?.Dispose();
        }

        public void ChangeActuators(int no, int inFlow, int outFlow, int throughFlow)
        {
            var config = ChambersDictionary[no].Configuration;
            var actuators = new int[3];
            actuators[config.InFlowActuatorNo - 1] = inFlow;
            actuators[config.OutFlowActuatorNo - 1] = outFlow;
            actuators[config.ThroughFlowActuatorNo - 1] = throughFlow;
            var chamber = ChambersDictionary[no];
            chamber.Sets.InFlow = inFlow;
            chamber.Sets.OutFlow = outFlow;
            chamber.Sets.ThroughFlow = throughFlow;
            controllersCommunicator.SendActuators(no, actuators[0], actuators[1], actuators[2]);
        }

        public void ChangeWent(int no, int value)
        {
            ChambersDictionary[no].Sets.Special = value;
            controllersCommunicator.SendSpecial(Wents[no], value);
        }

        public void ChangeRoof(int no, bool isRoof)
        {
            var roofNo = Roofs[no].RoofNo;
            var throughNo = Roofs[no].ThroughNo;
            var roofSet = isRoof ? 480 : 0;
            var throughSet = isRoof ? 0 : 480;

            ChambersDictionary[roofNo].Sets.Special = roofSet;
            ChambersDictionary[throughNo].Sets.Special = throughSet;

            controllersCommunicator.SendSpecial(roofNo, roofSet);
            controllersCommunicator.SendSpecial(throughNo, throughSet);
        }

        public void StopAll()
        {
            controllersCommunicator.StopAllActuators();
        }

        public CommonStatus GetCommon()
        {
            return new CommonStatus
            {
                Direction = false,
                InQueue = 0,
                TurnedOn = 0,
                WorkingNow = 0,
            };
        }

        public void ChangeChamberReading(int no, bool value)
        {
            ChambersDictionary[no].Listen = value;
        }

        public HistoryResponse GetHistory(int no, DateTime from, DateTime to)
        {
            var sensorsHistory = hisctoricalPersistance.GetSensorsHistory(no, from, to);
            var statusHistory = hisctoricalPersistance.GetStatusHistory(no, from, to);

            return new HistoryResponse {
                no = no,
                sensors = sensorsHistory,
                status = statusHistory,
            };
        }


        record RoofConfig
        {
            public int No { get; set; }
            public int RoofNo { get; set; }
            public int ThroughNo { get; set; }
        }

        record AdditionalConfig
        {
            public int? RoofRoof {get;set;}
            public int? RoofThrough {get;set;}
            public int? Went {get;set;}
        }
    }
}