using static Dryer_Server.Interfaces.ChamberConvertedStatus;

namespace Dryer_Server.Interfaces
{
    public record AdditionalStatus
    {
        public WorkingStatus Working { get; set; }
        public bool IsAuto { get; set; }
        public int? QueuePosition { get; set; }
        public int Position { get; set; }
        public int Set { get; set; }
    }
}