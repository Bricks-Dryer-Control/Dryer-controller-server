using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public record HistoryResponse
    {
        public int no { get; set; }
        public IEnumerable<IChamberSensorHistoricValue> sensors { get; set; }
        public IEnumerable<IChamberStatusHistoricValue> status { get; set; }
    }
}