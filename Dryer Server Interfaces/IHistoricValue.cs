using System;

namespace Dryer_Server.Interfaces
{
    public interface IHistoricValue
    {
        public int ChamberId { get; }
        public DateTime TimestampUtc { get; }
        bool DataEquals(IHistoricValue other);
    }
}