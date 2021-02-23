using System;
using System.Collections.Generic;
using Dryer_Server.Interfaces;

namespace Dryer_Server.Core
{
    public partial class Main
    {
        private class Chamber : IValueReceiver<ChamberSensors>, IValueReceiver<ChamberControllerStatus>
        {
            private int Id => Configuration.Id;
            private Main parrent;
            public ChamberConfiguration Configuration { get; }
            public AdditionalConfig additionalConfig { get; }

            public Chamber(ChamberConfiguration configuration, Main parrent, AdditionalConfig additional)
            {
                Configuration = configuration;
                this.parrent = parrent;
                this.additionalConfig = additionalConfig;
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
                var exs = new List<Exception>();
                var status = ConvertStatus(v);

                TrySave(exs, status);
                TrySendToUi(exs, status);

                if (additionalConfig?.RoofRoof != null)
                    TrySendRoofRoofToUi(exs, additionalConfig.RoofRoof.Value, v.Current4, v.Setted4);
                if (additionalConfig?.RoofThrough != null)
                    TrySendRoofThroughToUi(exs, additionalConfig.RoofThrough.Value, v.Current4, v.Setted4);
                if (additionalConfig?.Went != null)
                    TrySendWentToUi(exs, additionalConfig.Went.Value, v.Current4, v.Setted4);

                parrent.HandleExceptions(exs);
            }

            private void TrySendWentToUi(List<Exception> exs, int no, int position, int set)
            {
                try
                {
                    parrent.ui.WentChanged(no, position, set);
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            private void TrySendRoofThroughToUi(List<Exception> exs, int no, int position, int set)
            {
                try
                {
                    parrent.ui.RoofThroughChanged(no, position, set);
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
            }

            private void TrySendRoofRoofToUi(List<Exception> exs, int no, int position, int set)
            {
                try
                {
                    parrent.ui.RoofRoofChanged(no, position, set);
                }
                catch (Exception e)
                {
                    exs.Add(e);
                }
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
                var sets = new int[] { v.Setted1, v.Setted2, v.Setted3 };

                return new ChamberConvertedStatus
                {
                    Working = ChamberConvertedStatus.WorkingStatus.waiting,
                    IsAuto = false,
                    QueuePosition = null,
                    InFlowPosition = positions[Configuration.InFlowActuatorNo - 1],
                    OutFlowPosition = positions[Configuration.OutFlowActuatorNo - 1],
                    ThroughFlowPosition = positions[Configuration.ThroughFlowActuatorNo - 1],
                    InFlowSet = sets[Configuration.InFlowActuatorNo - 1],
                    OutFlowSet = sets[Configuration.OutFlowActuatorNo - 1],
                    ThroughFlowSet = sets[Configuration.ThroughFlowActuatorNo - 1],
                };
            }
        }
    }
}
