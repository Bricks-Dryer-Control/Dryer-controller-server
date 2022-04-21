using System;

namespace Dryer_Server.Interfaces
{
    public record AutoControlStatus
    {
        public string Name { get; set; }
        public TimeSpan CurrentTime { get; set; }
    }
}