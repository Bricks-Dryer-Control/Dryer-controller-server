using System;

namespace Dryer_Server.Interfaces
{
    public record AutoControlItem
    {
        public TimeSpan Time { get; set; }
        public float Temperature { get; set; }
        public int InFlow { get; set; }
        public int OutFlow { get; set; }
        public int ThroughFlow { get; set; }
    }
}