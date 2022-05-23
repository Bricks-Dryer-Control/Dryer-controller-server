namespace Dryer_Server.Interfaces
{
    public record ChamberConvertedStatus
    {
        public enum WorkingStatus
        {
            off, waiting, queued, working, addon,
            error
        }
        public WorkingStatus Working { get; set; }
        public bool IsAuto { get; set; }
        public int? QueuePosition { get; set; }
        public int InFlowPosition { get; set; }
        public int OutFlowPosition { get; set; }
        public int ThroughFlowPosition { get; set; }
        public int InFlowSet { get; set; }
        public int OutFlowSet { get; set; }
        public int ThroughFlowSet { get; set; }
        public bool IsListening { get; set; }
        public int OutFlowOffset { get; set; }
    }
}