using System;

namespace Dryer_Server.WebApi.Model
{
    public record CommonStatus
    {
        public int TurnedOn { get; set; }
        public int WorkingNow { get; set; }
        public int InQueue { get; set; }
        public bool Direction { get; set; }
    }
}