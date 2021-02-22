namespace Dryer_Server.Interfaces
{
    public record ChamberConfiguration
    {
        public int Id { get; set; }
        public int? SensorId { get; set; }
        public int InFlowActuatorNo { get; set; }
        public int OutFlowActuatorNo { get; set; }
        public int ThroughFlowActuatorNo { get; set; }
    }
}