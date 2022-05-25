using Dryer_Server.Interfaces;
using Dryer_Server.Persistance;
using Dryer_Server.Serial_Modbus_Agent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dryer_Server.Core
{
    public partial class Main : IMainController, IDisposable
    {
        private readonly IUiInterface ui;
        private readonly ISerialModbusChamberListener modbusListener;
        private readonly IDryerConfigurationPersistance configurationPersistance;
        private readonly IDryerHisctoricalValuesPersistance hisctoricalPersistance;
        private readonly IModbusControllerCommunicator controllersCommunicator;
        private readonly IAutoControlPersistance autoControlPersistence;

        private Dictionary<int, Chamber> ChambersDictionary { get; } = new Dictionary<int, Chamber>();

        private Dictionary<int, RoofConfig> Roofs { get; } = new Dictionary<int, RoofConfig>
        {
            {1, new RoofConfig{ No = 1, RoofNo = 4, ThroughNo = 5 }},
            {2, new RoofConfig{ No = 2, RoofNo = 6, ThroughNo = 7 }},
            {3, new RoofConfig{ No = 3, RoofNo = 8, ThroughNo = 9 }},
            {4, new RoofConfig{ No = 4, RoofNo = 15, ThroughNo = 14 }},
        };
        private Dictionary<int, WentConfig> Wents { get; } = new Dictionary<int, WentConfig>
        {
            {1, new WentConfig{ No = 1, WentNo = 16} },
            {2, new WentConfig{ No = 2, WentNo = 17} },
        };

        public Main(IUiInterface ui, 
            IDryerConfigurationPersistance dryerConfigurationPersistance, 
            IDryerHisctoricalValuesPersistance dryerHisctoricalValuesPersistance, 
            IAutoControlPersistance autoControlPersistance,
            ISerialModbusChamberListener serialModbusChamberListener, 
            IModbusControllerCommunicator modbusControllerCommunicator)
        {
            this.ui = ui;
            configurationPersistance = dryerConfigurationPersistance;
            hisctoricalPersistance = dryerHisctoricalValuesPersistance;
            autoControlPersistence = autoControlPersistance;
            modbusListener = serialModbusChamberListener;
            controllersCommunicator = modbusControllerCommunicator;
        }

        public Main(IUiInterface ui, SqlitePersistanceManager persistanceManager, PortSettings listenerPort, PortSettings controllersPort, DirSensorSettings dirSensor)
            : this(ui, 
                  persistanceManager, 
                  persistanceManager, 
                  persistanceManager,
                  new SerialModbusChamberListener(listenerPort), 
                  new ControllersCommunicator(controllersPort, dirSensor))
        { }

        public async Task InitializeAsync()
        {
            var chambers = configurationPersistance.GetChamberConfigurations();
            var ids = chambers.Select(c => c.Id).ToList();

            var lastValues = hisctoricalPersistance.GetLastValues(ids);
            var autoControls = autoControlPersistence.LoadStates();

            SetUpChambers(chambers, lastValues, autoControls);
            InitializeStatuses(lastValues);
            InitializeAutoControls(autoControls);

            var initData = ChambersDictionary.Values
                .Select(id => GetInitData(id, lastValues));

            await ui.InitializationFinishedAsync(initData, Wents.Count, Roofs.Count);
        }

        private ChamberInitializationData GetInitData(Chamber c, IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> lastValues)
        {
            var lastValue = lastValues.FirstOrDefault(v => v.id == c.Configuration.Id);

            return new ChamberInitializationData { 
                Id = c.Configuration.Id,
                Status = lastValue.status,
                Sensors = lastValue.sensors,
                AutoControl = c.CurrentAutoControl,
            };
        }

        private void SetUpChambers(IEnumerable<ChamberConfiguration> chambers, IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> lastValues, IEnumerable<(int chamberId, DateTime startUtc, AutoControl autoControl)> autoControls)
        {
            foreach (var chamberSettings in chambers)
            {
                var aditional = GetAdditionalConfig(chamberSettings);
                var autoControl = GetAutoControll(autoControls, chamberSettings.Id);

                var chamber = new Chamber(chamberSettings, this, aditional, autoControl);
                SetupCommunication(lastValues, chamberSettings, chamber);
                ChambersDictionary.Add(chamberSettings.Id, chamber);
            }
        }

        private AdditionalConfig GetAdditionalConfig(ChamberConfiguration chamberSettings)
        {
            return new AdditionalConfig
            {
                RoofRoof = Roofs.Values
                                    .Where(r => r.RoofNo == chamberSettings.Id)
                                    .FirstOrDefault()?.No,
                RoofThrough = Roofs.Values
                                    .Where(r => r.ThroughNo == chamberSettings.Id)
                                    .FirstOrDefault()?.No,
                Went = Wents.Values
                                    .Where(w => w.WentNo == chamberSettings.Id)
                                    .FirstOrDefault()?.No,
            };
        }

        private void SetupCommunication(IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> lastValues, ChamberConfiguration chamberSettings, Chamber chamber)
        {
            if (chamberSettings.SensorId.HasValue)
                modbusListener.Add(chamberSettings.SensorId.Value, chamber);

            var lastChamberValues = lastValues.FirstOrDefault(lv => lv.id == chamberSettings.Id);
            var isActive = lastChamberValues.status?.IsListening ?? false;
            controllersCommunicator.Register(chamberSettings, isActive, chamber);
        }

        private void InitializeStatuses(IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> lastValues)
        {
            foreach (var v in lastValues)
            {
                if (v.status != null && ChambersDictionary.TryGetValue(v.id, out var chamber))
                    chamber.InitializeStatus(v.status);
            }
        }

        private void InitializeAutoControls(IEnumerable<(int chamberId, DateTime startUtc, AutoControl autoControl)> autoControls)
        {
            var autoControlledChamberIds = autoControls.Select(a => a.chamberId);
            foreach (var id in autoControlledChamberIds)
            {
                if (ChambersDictionary.TryGetValue(id, out var chamber))
                    chamber.InitializeAutoControl();
            }
        }

        private Dryer_Auto_Control.AutoControl GetAutoControll(IEnumerable<(int chamberId, DateTime startUtc, AutoControl autoControl)> autoControls, int id)
        {
            try
            {
                var existing = autoControls
                    .Select(x => new (int chamberId, DateTime startUtc, AutoControl autoControl)?(x))
                    .FirstOrDefault(x => x.Value.chamberId == id);
                if (existing.HasValue)
                {
                    (int chamberId, DateTime startUtc, AutoControl autoControl) = existing.Value;
                    return Dryer_Auto_Control.AutoControl.NewAutoControl(autoControl, startUtc, null);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Could not load auto control for {id}\n{ex}");
            }

            return null;
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
            foreach (var chamber in ChambersDictionary.Values)
                chamber.DisposeAutoControl();
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
            var chamber = ChambersDictionary[no];
            var actuators = chamber.SetValuesGetActuators(inFlow, outFlow, throughFlow);
            controllersCommunicator.SendActuators(no, actuators[0], actuators[1], actuators[2]);
        }

        public void ChangeWent(int no, int value)
        {
            ChambersDictionary[no].Sets.Special = value;
            controllersCommunicator.SendSpecial(Wents[no].WentNo, value);
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

        public void EnqueueAutoControl(int no, IAutoValueGetter valueGetter)
        {
            controllersCommunicator.SendAuto(no, valueGetter);
        }

        public void StopAll()
        {
            controllersCommunicator.StopAllActuators();
        }

        public CommonStatus GetCommon()
        {
            return controllersCommunicator.GetCommonStatus();
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

        public void StartAutoControl(int chamberId, string name, TimeSpan startPoint)
        {
            var chamber = ChambersDictionary[chamberId];
            var startUtc = DateTime.UtcNow - startPoint;
            var autoControl = autoControlPersistence.Load(name);
            chamber.StartNewAutomaticControl(autoControl, startUtc);
            autoControlPersistence.SaveState(chamberId, chamber.CurrentAutoControl);
        }

        public void TurnAutoControl(int no, bool value)
        {
            var chamber = ChambersDictionary[no];
            if (value)
                chamber.CurrentAutoControl?.Start();
            else
                chamber.CurrentAutoControl?.Stop();
            chamber.SetAuto(value);
        }

        record RoofConfig
        {
            public int No { get; set; }
            public int RoofNo { get; set; }
            public int ThroughNo { get; set; }
        }

        record WentConfig
        {
            public int No { get; set; }
            public int WentNo { get; set; }
        }

        record AdditionalConfig
        {
            public int? RoofRoof {get;set;}
            public int? RoofThrough {get;set;}
            public int? Went {get;set;}
        }
    }
}