using System;

namespace Dryer_Server.WebApi.Model
{
    public record AutoControlItem
    {
        public double TimeSeconds { get; set; }
        public float Temperature { get; set; }
        public int InFlow { get; set; }
        public int OutFlow { get; set; }
        public int ThroughFlow { get; set; }

        public static implicit operator AutoControlItem(Interfaces.AutoControlItem autoControlItem)
        {
            return new AutoControlItem
            {
                TimeSeconds = autoControlItem.Time.TotalSeconds,
                Temperature = autoControlItem.Temperature,
                InFlow = autoControlItem.InFlow,
                OutFlow = autoControlItem.OutFlow,
            };
        }

        public static implicit operator Interfaces.AutoControlItem(AutoControlItem autoControlItem)
        {
            return new Interfaces.AutoControlItem
            {
                Time = TimeSpan.FromSeconds(autoControlItem.TimeSeconds),
                Temperature = autoControlItem.Temperature,
                InFlow = autoControlItem.InFlow,
                OutFlow = autoControlItem.OutFlow,
            };
        }
    }
}
