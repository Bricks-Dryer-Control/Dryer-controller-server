using Dryer_Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dryer_Server.WebApi.Model
{
    public record AutoControl
    {
        public string Name { get; set; }
        public double TimeToSetSeconds { get; set; }
        public float TemperatureDifference { get; set; }
        public int ControlDifference { get; set; }
        public AutoControlType ControlType { get; set; }
        public float Kp { get; set; }
        public float Ki { get; set; }
        public int MinInFlow { get; set; }
        public int MaxInFlow { get; set; }
        public int MinOutFlow { get; set; }
        public int MaxOutFlow { get; set; }
        public float Percent { get; set; }
        public int Offset { get; set; }
        public IEnumerable<AutoControlItem> Sets { get; set; }

        public static implicit operator AutoControl(Interfaces.AutoControl autoControl)
        {
            return new AutoControl
            {
                Name = autoControl.Name,
                TimeToSetSeconds = autoControl.TimeToSet.TotalSeconds,
                ControlDifference = autoControl.ControlDifference,
                ControlType = autoControl.ControlType,
                Kp = autoControl.Kp,
                Ki = autoControl.Ki,
                MinInFlow = autoControl.MinInFlow,
                MaxInFlow = autoControl.MaxInFlow,
                MinOutFlow = autoControl.MinOutFlow,
                MaxOutFlow = autoControl.MaxOutFlow,
                Percent = autoControl.Percent,
                Offset = autoControl.Offset,
                Sets = autoControl.Sets.Select(s => (AutoControlItem)s),
            };
        }

        public static implicit operator Interfaces.AutoControl(AutoControl autoControl)
        {
            return new Interfaces.AutoControl
            {
                Name = autoControl.Name,
                TimeToSet = TimeSpan.FromSeconds(autoControl.TimeToSetSeconds),
                ControlDifference = autoControl.ControlDifference,
                ControlType = autoControl.ControlType,
                Kp = autoControl.Kp,
                Ki = autoControl.Ki,
                MinInFlow = autoControl.MinInFlow,
                MaxInFlow = autoControl.MaxInFlow,
                MinOutFlow = autoControl.MinOutFlow,
                MaxOutFlow = autoControl.MaxOutFlow,
                Percent = autoControl.Percent,
                Offset = autoControl.Offset,
                Sets = autoControl.Sets.Select(s => (Interfaces.AutoControlItem)s).ToList(),
            };
        }
    }
}
