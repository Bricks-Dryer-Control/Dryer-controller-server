using Dryer_Server.Interfaces;
using System;
using AutoControlData = Dryer_Server.Interfaces.AutoControl;

namespace Dryer_Server.Dryer_Auto_Control
{
    public abstract class AutoControl : IAutoControl
    {
        protected readonly AutoControlData autoControlData;
        protected Func<int, int, int, int[]> SetValuesGetActuators;
        public string Name => autoControlData.Name;
        public DateTime StartDateUtc { get; protected set; }
        public abstract bool Active { get; }
        public abstract bool ShouldSend { get; }

        public static AutoControl NewAutoControl(AutoControlData autoControlData, DateTime startUtc, Func<ChamberConvertedStatus> getActualStatus = null, Func<ChamberSensors> getActualSsensors = null)
        {
            switch (autoControlData.ControlType)
            {
                case AutoControlType.PedefinedSettings:
                    return new PedefinedSettingsAutoControl(autoControlData, startUtc, getActualStatus);
                default:
                    return null;
            }
        }

        public AutoControl(AutoControlData autoControlData, DateTime startDateUtc)
        {
            this.autoControlData = autoControlData ?? throw new ArgumentNullException(nameof(autoControlData));
            StartDateUtc = startDateUtc;
        }

        public abstract void Start();
        public abstract void Stop();
        public abstract void Dispose();
        public abstract int[] GetValues();

        public void SetUpSetValuesGetActuators(Func<int, int, int, int[]> setValuesGetActuators)
        {
            SetValuesGetActuators = setValuesGetActuators;
        }
    }
}
