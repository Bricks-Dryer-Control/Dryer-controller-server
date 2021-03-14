namespace Dryer_Server.Interfaces
{
    public record ChamberControllerStatus
    {
        public enum WorkingStatus
        {
            Off = -1,
            NoOperation = 0,
            DirectionDetected = 1,
            ActuatorStarted = 2,
            ActuatorFinished = 3
        }

        public int ActualActuator { get; set; }
        public WorkingStatus workingStatus { get; set; }
        public int? QueuePosition { get; set; }
        public int Current1 { get; set; }
        public int Current2 { get; set; }
        public int Current3 { get; set; }        
        public int Current4 { get; set; }
    }
}