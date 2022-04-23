using System;

namespace Dryer_Server.Interfaces
{
    public record AutoControlStatus
    {
        private readonly DateTime startDateUtc;
        public AutoControlStatus(DateTime startUtc)
        {
            startDateUtc = startUtc;
        }

        public string Name { get; set; }
        public TimeSpan CurrentTime => DateTime.UtcNow - startDateUtc;
    }
}