namespace Dryer_Server.WebApi.Model
{
    public record ChamberValues
    {
        public int InFlow { get; set; }
        public int OutFlow { get; set; }
        public int ThroughFlow { get; set; }
    }
}