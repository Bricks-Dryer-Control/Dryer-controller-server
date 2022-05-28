using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public record HistoryResponse
    {
        public int no { get; set; }
        public IEnumerable<IHistoricValue> sensors { get; set; }
        public IEnumerable<IHistoricValue> status { get; set; }
    }
}