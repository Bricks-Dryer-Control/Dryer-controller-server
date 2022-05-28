using System;

namespace Dryer_Server.Persistance
{
    internal partial class HistoricalManager
    {
        private sealed record HistorySettings
        {
            public bool LogDifferencesOnly { get; set; }
            public TimeSpan LogTimeWindow { get; set; }
            public TimeSpan MaxTimeWithoutLog { get; set; }
        }
    }
}