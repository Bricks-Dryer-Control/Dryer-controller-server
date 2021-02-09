using System;

namespace Dryer_Server.Interfaces
{
    public interface IChamberSetting
    {        
        int ChamberId { get; set; }
        DateTime CreationTimeUtc { get; set; }
        int? SensorId { get; set; }
        int InflowIndex { get; set; }
        int OutflowIndex { get; set; }
        int ThroughflowIndex { get; set; }
        double Offset { get; set; }
        double K { get; set; }
        double Ti { get; set; }
        double Td { get; set; }
        double A { get; set; }
        double B { get; set; }
    }
}
