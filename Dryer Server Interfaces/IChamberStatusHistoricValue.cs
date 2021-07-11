using System;

namespace Dryer_Server.Interfaces
{
    public interface IChamberStatusHistoricValue
    {
        DateTime TimeUtc { get; }
        ChamberConvertedStatus Value { get; }
    }
}