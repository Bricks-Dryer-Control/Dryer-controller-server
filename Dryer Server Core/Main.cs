using Dryer_Server.Interfaces;
using Dryer_Server.Serial_Modbus_Agent;
using Dryer_Server.Persistance;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Dryer_Server.Core
{
    public partial class Main : IMainController, IDisposable
    {
        private readonly IUiInterface ui;
        private readonly SerialModbusChamberListener modbusListener;
        private readonly IDryerConfigurationPersistance configurationPersistance;
        private readonly IDryerHisctoricalValuesPersistance hisctoricalPersistance;
        private readonly IModbusControllerCommunicator controllersCommunicator;

        private Dictionary<int, Chamber> Chambers { get; } = new Dictionary<int, Chamber>();
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
            modbusListener = new SerialModbusChamberListener("COM1");
            var persistance = new SqlitePersistanceManager();
            configurationPersistance = persistance;
            hisctoricalPersistance = persistance;
            controllersCommunicator = new ControllersCommunicator();
        }

        public async Task InitializeAsync()
        {
            var chambers = configurationPersistance.GetChamberConfigurations();
            var ids = chambers.Select(c => c.Id).ToList();

            var lastValues = Task.Run(() => hisctoricalPersistance.GetLastValues(ids));
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

                var chamber = new Chamber(chamberSettings, this, aditional);
                if (chamberSettings.SensorId.HasValue)
                    modbusListener.Add(chamberSettings.SensorId.Value, chamber);
                controllersCommunicator.Register(chamberSettings, chamber);
                Chambers.Add(chamberSettings.Id, chamber);
            }
            await ui.InitializationFinishedAsync(await lastValues, initWents, initRoofs);
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
            var config = Chambers[no].Configuration;
            var actuators = new int[3];
            actuators[config.InFlowActuatorNo - 1] = inFlow;
            actuators[config.OutFlowActuatorNo - 1] = outFlow;
            actuators[config.ThroughFlowActuatorNo - 1] = throughFlow;
            controllersCommunicator.SendActuators(no, actuators[0], actuators[1], actuators[2]);
        }

        public void ChangeWent(int no, int value)
        {
            controllersCommunicator.SendSpecial(Wents[no], value);
        }

        public void ChangeRoof(int no, bool isRoof)
        {
            controllersCommunicator.SendSpecial(Roofs[no].RoofNo, isRoof ? 480 : 0);
            controllersCommunicator.SendSpecial(Roofs[no].ThroughNo, isRoof ? 0 : 480);
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