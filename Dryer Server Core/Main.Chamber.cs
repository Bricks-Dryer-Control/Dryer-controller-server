using Dryer_Server.Interfaces;
using System;
using System.Collections.Generic;
using static Dryer_Server.Interfaces.ChamberConvertedStatus;

namespace Dryer_Server.Core
{
    public partial class Main
    {
        private class Chamber : IValueReceiver<ChamberSensors>, IValueReceiver<ChamberControllerStatus>, IAutoControlledChamber
        {
            private int Id => Configuration.Id;
            private ChamberConvertedStatus currentStatus;
            private readonly Main parrent;
            public ChamberValues Sets { get; } = new ChamberValues();
            public ChamberConfiguration Configuration { get; }
            public AdditionalConfig AdditionalConfig { get; }
            public IAutoControl CurrentAutoControl { get; private set; }

            public bool Listen 
            { 
                get => isListen(Id); 
                set => setListen(Id, value);
            }

            ChamberConvertedStatus IAutoControlledChamber.ConvertedStatus => currentStatus;

            ChamberSensors IAutoControlledChamber.ChamberSensors => throw new NotImplementedException();

            public int OutFlowOffset => Configuration.OutFlowOffset;

            public bool? DirSensor => parrent.DirSensor;

            public Chamber(ChamberConfiguration configuration, Main parrent, AdditionalConfig additional, IAutoControl autoControl)
            {
                Configuration = configuration;
                this.parrent = parrent;
                AdditionalConfig = additional;
                CurrentAutoControl = autoControl;
                autoControl?.SetChamber(this);
            }

            public void ValueReceived(ChamberSensors v)
            {
                var exs = new List<Exception>();
                TrySave(v, exs);
                TrySendToUi(v, exs);

                parrent.HandleExceptions(exs);
            }

            private void TrySendToUi(ChamberSensors v, List<Exception> exs)
            {
                try
                {
                    parrent.ui.SensorsReceived(Id, DateTime.UtcNow, v);
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            private void TrySave(ChamberSensors v, List<Exception> exs)
            {
                try
                {
                    parrent.hisctoricalPersistance.Save(Id, v);
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            public void ValueReceived(ChamberControllerStatus v)
            {
                if (v.workingStatus != ChamberControllerStatus.WorkingStatus.Error)
                {
                    var exs = new List<Exception>();
                    currentStatus = ConvertStatus(v);
                    currentStatus.OutFlowOffset = Configuration.OutFlowOffset;

                    TrySave(exs, currentStatus);
                    TrySendToUi(exs, currentStatus);
                    

                    if (AdditionalConfig?.RoofRoof != null)
                        TrySendRoofRoofToUi(exs, AdditionalConfig.RoofRoof.Value, v);
                    if (AdditionalConfig?.RoofThrough != null)
                        TrySendRoofThroughToUi(exs, AdditionalConfig.RoofThrough.Value, v);
                    if (AdditionalConfig?.Went != null)
                        TrySendWentToUi(exs, AdditionalConfig.Went.Value, v);

                    parrent.HandleExceptions(exs);
                }
                else 
                {
                    parrent.ui.StatusChanged(Id, DateTime.UtcNow, new ChamberConvertedStatus
                    {
                        Working = WorkingStatus.error,
                    });
                }
            }

            private void TrySendWentToUi(List<Exception> exs, int no, ChamberControllerStatus status)
            {
                try
                {
                    parrent.ui.WentChanged(no, status.Current4, Sets.Special, status.QueuePosition, GetStatus(status));
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            private void TrySendRoofThroughToUi(List<Exception> exs, int no, ChamberControllerStatus status)
            {
                try
                {
                    parrent.ui.RoofThroughChanged(no, status.Current4, Sets.Special, status.QueuePosition, GetStatus(status));
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            private void TrySendRoofRoofToUi(List<Exception> exs, int no, ChamberControllerStatus status)
            {
                try
                {
                    parrent.ui.RoofRoofChanged(no, status.Current4, Sets.Special, status.QueuePosition, GetStatus(status));
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            internal void InitializeStatus(ChamberConvertedStatus status)
            {
                Sets.InFlow = status.InFlowSet;
                Sets.OutFlow = status.OutFlowSet;
                Sets.ThroughFlow = status.ThroughFlowSet;
                status.OutFlowOffset = Configuration.OutFlowOffset;
                currentStatus = status;
            }

            private WorkingStatus GetStatus(ChamberControllerStatus status)
            {
                if (status.workingStatus == ChamberControllerStatus.WorkingStatus.Off)
                    return WorkingStatus.off;
                
                if (status.workingStatus != ChamberControllerStatus.WorkingStatus.NoOperation)
                {
                    if (status.ActualActuator == 5)
                        return WorkingStatus.addon;
                    return WorkingStatus.working;
                }

                return status.QueuePosition == null ? WorkingStatus.waiting : WorkingStatus.queued;
            }

            internal void InitializeAutoControl()
            {
                if (currentStatus != null && currentStatus.IsAuto)
                    CurrentAutoControl?.Start();
            }

            private void TrySendToUi(List<Exception> exs, ChamberConvertedStatus status)
            {
                try
                {
                    parrent.ui.StatusChanged(Id, DateTime.UtcNow, status);
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            private void TrySave(List<Exception> exs, ChamberConvertedStatus status)
            {
                try
                {
                    parrent.hisctoricalPersistance.Save(Id, status);
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            private ChamberConvertedStatus ConvertStatus(ChamberControllerStatus v)
            {
                var positions = new int[] { v.Current1, v.Current2, v.Current3 };

                return new ChamberConvertedStatus
                {
                    Working = GetStatus(v),
                    IsAuto = CurrentAutoControl?.Active ?? false,
                    QueuePosition = null,
                    InFlowPosition = positions[Configuration.InFlowActuatorNo - 1],
                    OutFlowPosition = positions[Configuration.OutFlowActuatorNo - 1],
                    ThroughFlowPosition = positions[Configuration.ThroughFlowActuatorNo - 1],
                    InFlowSet = Sets.InFlow,
                    OutFlowSet = Sets.OutFlow,
                    ThroughFlowSet = Sets.ThroughFlow,
                    IsListening = Listen,
                };
            }

            internal void DisposeAutoControl()
            {
                CurrentAutoControl?.Dispose();
            }

            private void setListen(int id, bool value)
            {
                parrent.controllersCommunicator.setChamberListen(id, value);
                parrent.ui.ActiveChanged(id, value);
            }

            private bool isListen(int id)
            {
                return parrent.controllersCommunicator.isChamberListen(id);
            }

            public void StartNewAutomaticControl(AutoControl autoControlData, DateTime startUtc)
            {
                CurrentAutoControl?.Dispose();
                CurrentAutoControl = Dryer_Auto_Control.AutoControl.NewAutoControl(autoControlData, startUtc, this);
                parrent.ui.AutoControlChanged(Id, CurrentAutoControl);
            }

            public void AutoControlChanged()
            {
                parrent.ui.AutoControlChanged(Id, CurrentAutoControl);
            }

            public int[] SetValuesGetActuators(int inFlow, int outFlow, int throughFlow)
            {
                var actuators = new int[3];

                actuators[Configuration.InFlowActuatorNo - 1] = inFlow;
                actuators[Configuration.OutFlowActuatorNo - 1] = outFlow;
                actuators[Configuration.ThroughFlowActuatorNo - 1] = throughFlow;

                Sets.InFlow = inFlow;
                Sets.OutFlow = outFlow;
                Sets.ThroughFlow = throughFlow;

                return actuators;
            }

            void IAutoControlledChamber.EnqueueAutoControl(IAutoValueGetter valueGetter)
            {
                parrent.EnqueueAutoControl(Id, valueGetter);
            }

            internal void SetAuto(bool value)
            {
                currentStatus.IsAuto = value;
                parrent.hisctoricalPersistance.Save(Id, currentStatus);
            }
        }
    }
}
