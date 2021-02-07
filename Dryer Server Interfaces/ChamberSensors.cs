namespace Dryer_Server.Interfaces
{
    public record ChamberSensors
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}