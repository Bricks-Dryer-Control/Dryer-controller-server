using System;
using Dryer_Server.Interfaces;

namespace Dryer_Server.WebApi.Model
{
    public record ChamberInfo
    {
        private static readonly DateTime startTimeUtc = new DateTime(1900, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        public ChamberInfo()
        { }

        public ChamberInfo(int id, ChamberConvertedStatus status, ChamberSensors sensors)
        {
            No = id;
            ReadingTime = startTimeUtc;
            ActualActuators = new ChamberValues
            {
                InFlow = status.InFlowPosition,
                OutFlow = status.OutFlowPosition,
                ThroughFlow = status.ThroughFlowPosition,
            };
            Humidity = sensors.Humidity;
            Temperature = sensors.Temperature;
            SetActuators = new ChamberValues
            {
                InFlow = status.InFlowSet,
                OutFlow = status.OutFlowSet,
                ThroughFlow = status.ThroughFlowSet,
            };
            Status = new ChamberStatus
            {
                IsAuto = false,
                QueuePosition = status.QueuePosition,
                Working = status.Working,
            };
        }

        public int No { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public DateTime ReadingTime { get; set; }
        public ChamberValues ActualActuators { get; set; }
        public ChamberValues SetActuators { get; set; }
        public ChamberStatus Status { get; set; }
    }
}