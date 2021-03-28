using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public record AutoControl
    {
        public string Name { get; set; }
        public TimeSpan TimeToSet { get; set; }
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
        public ICollection<AutoControlItem> Sets { get; set; }
    }
}