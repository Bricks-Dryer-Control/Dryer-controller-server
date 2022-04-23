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
    }
}
