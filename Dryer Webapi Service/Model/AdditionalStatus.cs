namespace Dryer_Server.WebApi.Model
{
    public record AdditionalStatus
    {
        public AdditionalStatus()
        { }

        public AdditionalStatus(Interfaces.AdditionalStatus iStatus)
        {
            actualValue = iStatus.Position;
            setValue = iStatus.Set;
            status = new ChamberStatus
            {
                IsAuto = iStatus.IsAuto,
                QueuePosition = iStatus.QueuePosition,
                Working = iStatus.Working,
            };
        }

        public int actualValue { get; set; }
        public int setValue { get; set; }
        public ChamberStatus status { get; set; }
    }
}