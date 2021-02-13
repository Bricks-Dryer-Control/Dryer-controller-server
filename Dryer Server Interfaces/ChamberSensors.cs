using System;

namespace Dryer_Server.Interfaces
{
    public record ChamberSensors
    {
        public float Temperature { get; set; }
        public float Humidity { get; set; }
    }
}