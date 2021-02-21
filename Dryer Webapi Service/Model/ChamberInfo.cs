using System;

namespace Dryer_Server.WebApi.Model
{
    public record ChamberInfo
    {
        public int No { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public DateTime ReadingTime { get; set; }
        public ChamberValues ActualActuators { get; set; }
        public ChamberValues SetActuators { get; set; }
        public ChamberStatus Status { get; set; }
    }
}