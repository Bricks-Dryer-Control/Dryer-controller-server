namespace Dryer_Server.Interfaces
{
    public record DirSensorSettings
    {
        public byte ControllerId { get; set; }
        public ushort InputNumber { get; set; }
    }
}
