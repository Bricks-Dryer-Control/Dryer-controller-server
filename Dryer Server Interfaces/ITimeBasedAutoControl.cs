using Dryer_Server.Interfaces;
using System;

namespace Dryer_Server_Interfaces
{
    public interface ITimeBasedAutoControl
    {
        DateTime StartMoment { get; }
        IAutoControlledChamber AutoControlledChamber { get; }
        AutoControl AutoControl { get; }
        TimeSpan CheckingDelay { get; }
    }
}
