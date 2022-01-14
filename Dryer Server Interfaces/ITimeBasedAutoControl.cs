using System;

namespace Dryer_Server.Interfaces
{
    public interface ITimeBasedAutoControl
    {
        DateTime StartMoment { get; }
        IAutoControlledChamber AutoControlledChamber { get; }
        AutoControl AutoControl { get; }
    }
}
