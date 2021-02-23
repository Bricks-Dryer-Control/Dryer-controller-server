namespace Dryer_Server.Interfaces
{
    public record AdditionalStatus
    {
        public enum WorkingStatus
        {
            off, waiting, queued, working, addon
        };

        public WorkingStatus Working { get; set; }
        public bool IsAuto { get; set; }
        public int? QueuePosition { get; set; }
        public int Position { get; set; }
        public int Set { get; set; }
    }
}