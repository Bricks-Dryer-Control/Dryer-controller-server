namespace Dryer_Server.Interfaces
{
    public record PortSettings
    {
        public string Port { get; set; }
        public int Baud { get; set; }
        public int DataBits { get; set; }
        public char Parity { get; set; }
        public int StopBits { get; set; }
    }
}