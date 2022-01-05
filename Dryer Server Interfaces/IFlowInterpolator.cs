using System;


namespace Dryer_Server.Interfaces
{
    public interface IFlowInterpolator
    {
        Flow InterpolateFlow();
    }

    public record Flow
    {
        public int InFlow { get; init; }
        public int OutFlow { get; init; }
        public int ThroughFlow { get; init; }
    }
}
