using Dryer_Server.Interfaces;
using System;
using System.Collections.Generic;
using Dryer_Server_Interfaces;
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
            public CahmberValues Sets { get; } = new CahmberValues();
            public ChamberConfiguration Configuration { get; }
            public AdditionalConfig AdditionalConfig { get; }
            public Boolean Listen 
            { 
                get => isListen(Id); 
                set => setListen(Id, value);
            }


            public Chamber(ChamberConfiguration configuration, Main parrent, AdditionalConfig additional)
            {
                Configuration = configuration;
                this.parrent = parrent;
                AdditionalConfig = additional;
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
                    Working = ChamberConvertedStatus.WorkingStatus.waiting,
                    IsAuto = false,
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

            private void setListen(int id, bool value)
            {
                parrent.controllersCommunicator.setChamberListen(id, value);
                parrent.ui.ActiveChanged(id, value);
            }

            private bool isListen(int id)
            {
                return parrent.controllersCommunicator.isChamberListen(id);
            }


            bool IAutoControlledChamber.IsAutoControl => currentStatus.IsAuto;
            bool IAutoControlledChamber.IsQueued => parrent.controllersCommunicator.IsChamberQueued(Configuration.Id);
            int IAutoControlledChamber.CurrentInFlow => Sets.InFlow;
            int IAutoControlledChamber.CurrentOutFlow => Sets.OutFlow;
            int IAutoControlledChamber.CurrentThroughFlow => Sets.ThroughFlow;

            int IAutoControlledChamber.Id => Configuration.Id;

            void IAutoControlledChamber.AddToQueue(IFlowInterpolator interpolator)
            {
                parrent.controllersCommunicator.SendTimeBased(Id, interpolator);
            }
        }
    }
}
