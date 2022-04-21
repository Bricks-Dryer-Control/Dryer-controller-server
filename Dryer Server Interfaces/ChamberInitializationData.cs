namespace Dryer_Server.Interfaces
{
    public record ChamberInitializationData
    {
        public int Id { get; set; }
        public ChamberConvertedStatus Status { get; set; }
        public ChamberSensors Sensors { get; set; }
        public IAutoControl AutoControl { get; set; }
        public int WentQty { get; set; }
        public int RoofQty { get; set; }
    }
}