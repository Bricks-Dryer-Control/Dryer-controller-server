using Dryer_Server.Interfaces;
using System;
using AutoControlData = Dryer_Server.Interfaces.AutoControl;

namespace Dryer_Server.Dryer_Auto_Control
{
    public abstract class AutoControl : IAutoControl
    {
        protected readonly AutoControlData autoControlData;
        protected IAutoControlledChamber chamber;
        public string Name => autoControlData.Name;
        public DateTime StartDateUtc { get; protected set; }
        public abstract bool Active { get; }
        public abstract bool ShouldSend { get; }

        public static AutoControl NewAutoControl(AutoControlData autoControlData, DateTime startUtc, IAutoControlledChamber chamber)
        {
            switch (autoControlData.ControlType)
            {
                case AutoControlType.PedefinedSettings:
                    return new PedefinedSettingsAutoControl(autoControlData, startUtc, chamber);
                default:
                    return null;
            }
        }

        public AutoControl(AutoControlData autoControlData, DateTime startDateUtc, IAutoControlledChamber chamber)
        {
            this.autoControlData = autoControlData ?? throw new ArgumentNullException(nameof(autoControlData));
            this.chamber = chamber;
            StartDateUtc = startDateUtc;
        }

        public abstract void Start();
        public abstract void Stop();
        public abstract void Dispose();
        public abstract int[] GetValues();

        public void SetChamber(IAutoControlledChamber chamber)
        {
            this.chamber = chamber;
        }

        protected void ManipulateOutFlow(ref int outFlow)
        {
            if (chamber.OutFlowOffset == 0)
            {
                return;
            }

            if (chamber.DirSensor == null)
            {
                outFlow = chamber.ConvertedStatus.OutFlowSet;
                return;
            }
            
            if (chamber.DirSensor == true)
            {
                outFlow = -outFlow;
            }

            outFlow += chamber.OutFlowOffset;
        }

        protected bool CheckSetValues(ref int inFlow, ref int outFlow, ref int throughFlow)
        {
            var d = autoControlData.ControlDifference;
            var status = chamber.ConvertedStatus;

            if (inFlow < autoControlData.MinInFlow)
                inFlow = autoControlData.MinInFlow;
            else if (inFlow > autoControlData.MaxInFlow)
                inFlow = autoControlData.MaxInFlow;
            if (inFlow + d < status.InFlowPosition || inFlow - d > status.InFlowPosition)
                return true;
            
            if (outFlow < autoControlData.MinOutFlow)
                outFlow = autoControlData.MinOutFlow;
            else if (outFlow > autoControlData.MaxOutFlow)
                outFlow = autoControlData.MaxOutFlow;
            if (outFlow + d < status.OutFlowPosition || outFlow - d > status.OutFlowPosition)
                return true;

            if (throughFlow + d < status.ThroughFlowPosition || throughFlow - d > status.ThroughFlowPosition)
                return true;

            return false;
        }
    }
}
