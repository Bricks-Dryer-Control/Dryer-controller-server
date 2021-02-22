namespace Dryer_Server.WebApi.Model
{
    public record AdditionalStatus
    {
        public int actualValue { get; set; }
        public int setValue { get; set; }
        public ChamberStatus status { get; set; }
    }
}